using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;

namespace FutureFeatureGenerator;
[DebuggerDisplay("{Name}")]
internal abstract class NodeBase
{
    public static readonly StringCache NodeNameCache = new();
    public readonly string Name;
    public readonly byte[] NameData;
    public string? AliasName;
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
    public virtual IReadOnlyList<NodeBase>? FindAllNode(string name) => null;
    public bool IsSibling(NodeBase node)
    {
        if (Parent is null || node.Parent is null)
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
    public void AddChild(string name, Stream contentStream, int skipCount, int startLine)
    {
        ThrowIfExists(name);
        Children.Add(new TempNodeLeaf(name, this, contentStream, skipCount, startLine) { Depth = Depth + 1 });
    }
    public void AddChild(string name, StreamReaderWrapper readerWrapper, int skipCount, int startLine)
    {
        ThrowIfExists(name);
        Children.Add(new TempNodeLeaf(name, this, readerWrapper, skipCount, startLine) { Depth = Depth + 1 });
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
    public override IReadOnlyList<NodeBase>? FindAllNode(string name)
    {
        if(name == "*")
        {
            return Children;
        }
        List<NodeBase>? result = null;
        foreach (var child in Children)
        {
            if(string.Equals(name, child.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(name, child.AliasName, StringComparison.OrdinalIgnoreCase))
            {
                (result ??= new()).Add(child);
            }
        }
        return result;
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
        if (FindNode(nodeBase.Name, out var node))
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
        for (int i = 0; i < count; i++)
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
    private static Regex nameofValueRegex = new Regex(@"nameof\((.*?)\)");
    public string Condition;
    public List<string> Dependencies = new();
    public List<NodeBase> NodeDependencies = new();
    public List<string> Lines = new();
    public TempNodeLeaf(string name, NodeBase parent, StreamReaderWrapper readerWrapper, int skipCount, int startLine) : base(name, false, parent)
    {
        const int ReadDependencies = 0, ReadCondition = 1, ReadLine = 2, EndCondition = 3;
        var state = 0;
        while (readerWrapper.CurrentLine < startLine)
        {
            readerWrapper.ReadLine();
        }
        string? text;
        var ifCount = 0;
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
                    var dependencyText = text.AsSpan(skipCount);
                    if (dependencyText.StartsWith("//".AsSpan()))
                    {
                        Dependencies.Add(text.AsSpan(skipCount + "//".Length).Trim().ToString());
                    }
                    else if (dependencyText.StartsWith("[RequireType".AsSpan()))
                    {
                        Dependencies.Add(nameofValueRegex.Match(text).Groups[1].Value);
                    }
                    else if (dependencyText.StartsWith("[Alias".AsSpan()))
                    {
                        AliasName = nameofValueRegex.Match(text).Groups[1].Value + "()";
                    }
                    else
                    {
                        state++;
                        goto case ReadCondition;
                    }
                    break;
                case ReadCondition:
                    Condition = text.Substring("#if".Length).Trim();
                    ifCount = 1;
                    state++;
                    break;
                case ReadLine:
                    if (text.StartsWith("#if"))
                    {
                        ifCount++;
                        Lines.Add(text);
                    }
                    else if (text.StartsWith("#endif"))
                    {
                        if (--ifCount == 0)
                        {
                            state++;
                            goto case EndCondition;
                        }
                        else
                        {
                            Lines.Add(text);
                        }
                    }
                    else
                    {
                        // #else
                        if (text[0] == '#')
                        {
                            Lines.Add(text);
                        }
                        else
                        {
                            Lines.Add(text.AsSpan(skipCount).ToString());
                        }

                    }
                    break;
                case EndCondition:
                    if (text.Trim() != "#endif")
                    {
                        throw new InvalidDataException("the last line must be '#endif'");
                    }
                    goto endRead;
                default:
                    throw new NotImplementedException("unknown state");
            }
        }
    endRead:
        if (state != EndCondition)
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
    public TempNodeLeaf(string name, NodeBase parent, Stream contentStream, int skipCount, int startLine) : this(name, parent, new StreamReader(contentStream).GetWrapper(), skipCount, startLine)
    {
    }
    public NodeLeaf GetNodeLeaf(StringCache condititonCache, NodeLeaf[] dependencies)
    {
        return new NodeLeaf(Name, Parent!, condititonCache.GetOrAdd(Condition), dependencies, Lines.ToArray()) { Depth = Depth, AliasName = AliasName };
    }
}
internal class NodeLeaf : NodeBase
{
    private static readonly Dictionary<string, Func<string[], bool>> condititonFuncCache = new();
    public string Condition;
    public Func<string[], bool> ConditionFunc;
    public NodeLeaf[] Dependencies;
    public string[] Lines;
    public int Order;
    public int ModiferLineIndex = -1;
    public NodeLeaf(string name, NodeBase parent, string condition, NodeLeaf[] dependencies, string[] lines) : base(name, false, parent ?? throw new ArgumentNullException(nameof(parent)))
    {
        Condition = condition;
        if (!condititonFuncCache.TryGetValue(condition, out var func))
        {
            func = Utils.GetConditionFunc(condition);
            condititonFuncCache.Add(condition, func);
        }
        ConditionFunc = func;
        Dependencies = dependencies;
        Lines = lines;
    }
    public NodeLeaf CloneWithNewParent(NodeBase parent)
    {
        var result = (NodeLeaf)MemberwiseClone();
        result.Parent = parent;
        return result;
    }
}
internal class NodeEqualityComparer : IEqualityComparer<NodeBase>
{
    // there may be bugs
    public bool Equals(NodeBase x, NodeBase y) => ReferenceEquals(x.Parent!.Name, y.Parent!.Name) && ReferenceEquals(x.Name, y.Name);

    public int GetHashCode(NodeBase obj) => obj.Name.GetHashCode();
}