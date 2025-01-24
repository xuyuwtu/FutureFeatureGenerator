using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Microsoft.CodeAnalysis.CSharp;

namespace FutureFeatureGenerator;
[DebuggerDisplay("{Name}")]
internal abstract class NodeBase
{
    public static readonly StringCache NodeNameCache = new();
    public readonly string Name;
    public readonly byte[] NameData;
    public int Depth;
    public bool HasChildren;
    public NodeBase? Parent;
    public NodeBase(string name, bool hasChildren, NodeBase? parent)
    {
        Name = name;
        NameData = NodeNameCache.GetOrAddAsBytes(name);
        HasChildren = hasChildren;
        Parent = parent;
    }
    public virtual NodeBase? FindNode(string name) => null;
    public bool IsSibling(NodeBase node)
    {
        if(Parent is null || node.Parent is null)
        {
            return false;
        }
        return ReferenceEquals(Parent, node.Parent);
    }
}
internal class NodeCommon : NodeBase, IEnumerable<NodeBase>
{
    public List<NodeBase> Children = new();
    protected string? WriteString { get; set; }
    public NodeCommon(string name, NodeBase? parent) : base(name, true, parent)
    {
    }
    public virtual string GetText(string modifer) => WriteString ??= $"namespace {Name}";
    private void ThrowIfExists(string name)
    {
        foreach (var child in Children)
        {
            if (string.Equals(name, child.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"'{name}' is exists");
            }
        }
    }
    public virtual NodeCommon AddChild(string name)
    {
        ThrowIfExists(name);
        var node = new NodeCommon(name, this)
        {
            Depth = Depth + 1
        };
        Children.Add(node);
        return node;
    }
    public void AddChild(string name, Stream contentStream, int startLine = 2)
    {
        ThrowIfExists(name);
        Children.Add(new TempNodeLeaf(name, this, contentStream, startLine) { Depth = Depth + 1 });
    }
    public void AddChild(string name, StreamReaderWrapper readerWrapper, int startLine = 2)
    {
        ThrowIfExists(name);
        Children.Add(new TempNodeLeaf(name, this, readerWrapper, startLine) { Depth = Depth + 1 });
    }
    public void AddChild(NodeBase node)
    {
        ThrowIfExists(node.Name);
        node.Depth = Depth + 1;
        Children.Add(node);
    }
    public override NodeBase? FindNode(string name)
    {
        foreach (var child in Children)
        {
            if (string.Equals(name, child.Name, StringComparison.OrdinalIgnoreCase))
            {
                return child;
            }
        }
        return null;
    }
    public bool FindNode(string name, [MaybeNullWhen(false)] out NodeBase node)
    {
        foreach (var child in Children)
        {
            if (string.Equals(name, child.Name, StringComparison.OrdinalIgnoreCase))
            {
                node = child;
                return true;
            }
        }
        node = null;
        return false;
    }
    public NodeCommon GetOrAddChild(NodeCommon nodeBase)
    {
        if(FindNode(nodeBase.Name, out var node))
        {
            return (NodeCommon)node;
        }
        var result = nodeBase.Clone();
        AddChild(result);
        return result;
    }
    protected virtual NodeCommon Clone() => new NodeCommon(Name, Parent);
    public List<NodeBase>.Enumerator GetEnumerator() => Children.GetEnumerator();
    IEnumerator<NodeBase> IEnumerable<NodeBase>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
internal class NodeRoot : NodeCommon
{
    public NodeRoot() : base("", null)
    {
        Depth = 0;
    }
    public override NodeCommon AddChild(string name)
    {
        var result = base.AddChild(name);
        result.Parent = null;
        return result;
    }
    public NodeBase? FindNodeByFullName(string fullName)
    {
        NodeBase? node = this;
        var nameList = fullName.Split(Utils.PointSeparator);
        var count = nameList.Length;
        for(int i = 0; i < count; i++)
        {
            if (node is null)
            {
                break;
            }
            node = node.FindNode(nameList[i]);
        }
        return node;
    }
}
internal class NodeClass : NodeCommon
{
    public NodeClass(string name, NodeBase? parent) : base(name, parent)
    {
    }
    public override string GetText(string modifer) => $"{modifer} static partial class Future{Name}";
    protected override NodeCommon Clone() => new NodeClass(Name, Parent);
}
internal class TempNodeLeaf : NodeBase
{
    public LanguageVersion LanguageVersion;
    public string Condition;
    public List<string> Dependencies = new();
    public List<NodeBase> NodeDependencies = new();
    public List<string> Lines = new();
    public TempNodeLeaf(string name, NodeBase parent, StreamReaderWrapper readerWrapper, int startLine = 2) : base(name, false, parent)
    {
        const int ReadDependencies = 0, ReadCondition = 1, ReadLine = 2, EndCondition = 3;
        var state = 0;
        while(readerWrapper.CurrentLine < startLine)
        {
            readerWrapper.ReadLine();
        }
        string? text = readerWrapper.ReadLine();
        var skipCount = text.IndexOf('/');
        LanguageVersion = Utils.GetLanguageVersion(text.AsSpan(skipCount + "//".Length).Trim());
        while (!readerWrapper.EndOfStream)
        {
            text = readerWrapper.ReadLine();
            if (string.IsNullOrEmpty(text))
            {
                continue;
            }
            switch (state)
            {
                case ReadDependencies:
                    if (text.AsSpan(skipCount).StartsWith("//".AsSpan()))
                    {
                        Dependencies.Add(text.AsSpan(skipCount + "//".Length).Trim().ToString());
                    }
                    else
                    {
                        state++;
                        goto case ReadCondition;
                    }
                    break;
                case ReadCondition:
                    Condition = text.Substring("#if".Length).Trim();
                    state++;
                    break;
                case ReadLine:
                    if (text.StartsWith("#endif"))
                    {
                        state++;
                        goto case EndCondition;
                    }
                    else
                    {
                        Lines.Add(text.AsSpan(skipCount).ToString());
                    }
                    break;
                case EndCondition:
                    if(text.Trim() != "#endif")
                    {
                        throw new InvalidDataException("the last line must be '#endif'");
                    }
                    goto endRead;
                default:
                    throw new NotImplementedException("unknown state");
            }
        }
        endRead:
        if(state != EndCondition)
        {
            throw new InvalidDataException("not found '^#endif'");
        }
        if (string.IsNullOrEmpty(Condition))
        {
            throw new ArgumentException("Condition cannot be null");
        }
        // resolve null warn
        Condition ??= "";
    }
    public TempNodeLeaf(string name, NodeBase parent, Stream contentStream, int startLine) : this(name, parent, new StreamReader(contentStream).GetWrapper(), startLine)
    {
    }
    public NodeLeaf GetNodeLeaf(StringCache condititonCache, NodeLeaf[] dependencies)
    {
        return new NodeLeaf(Name, Parent!, condititonCache.GetOrAdd(Condition), dependencies, Lines.ToArray()) { Depth = Depth, LanguageVersion = LanguageVersion };
    }
}
internal class NodeLeaf : NodeBase
{
    public LanguageVersion LanguageVersion;
    public string Condition;
    public NodeLeaf[] Dependencies;
    public string[] Lines;
    public int Order;
    public int ModiferLineIndex = -1;
    public NodeLeaf(string name, NodeBase parent, string condition, NodeLeaf[] dependencies, string[] lines) : base(name, false, parent ?? throw new ArgumentNullException(nameof(parent)))
    {
        Condition = condition;
        Dependencies = dependencies;
        Lines = lines;
    }
    public NodeLeaf CloneWithNewParent(NodeBase parent) => new(Name, parent, Condition, Dependencies, Lines) { Depth = Depth, LanguageVersion = LanguageVersion, Order = Order, ModiferLineIndex = ModiferLineIndex };
}
internal class NodeEqualityComparer : IEqualityComparer<NodeBase>
{
    // there may be bugs
    public bool Equals(NodeBase x, NodeBase y) => ReferenceEquals(x.Parent!.Name, y.Parent!.Name) && ReferenceEquals(x.Name, y.Name);

    public int GetHashCode(NodeBase obj) => obj.Name.GetHashCode();
}