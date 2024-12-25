using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Microsoft.CodeAnalysis.CSharp;

namespace FutureFeatureGenerator;
[DebuggerDisplay("{Name}")]
internal abstract class NodeBase
{
    public string Name;
    public int Depth;
    public bool HasChildren;
    public NodeBase? Parent;
    public NodeBase(string name, bool hasChildren, NodeBase? parent)
    {
        Name = name;
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

    public NodeCommon(string name, NodeBase? parent) : base(name, true, parent)
    {
    }
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
    public void AddChild(string name, Stream contentStream)
    {
        ThrowIfExists(name);
        Children.Add(new TempNodeLeaf(name, this, contentStream) { Depth = Depth + 1 });
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
    public NodeCommon GetOrAddChild(string name)
    {
        if(FindNode(name, out var node))
        {
            return (NodeCommon)node;
        }
        return AddChild(name);
    }
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
        if (!fullName.EndsWith(".cs"))
        {
            return null;
        }
        NodeBase? node = this;
        var nameList = fullName.Split(Utils.Separator);
        var count = nameList.Length - 1;
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
internal class TempNodeLeaf : NodeBase
{
    public LanguageVersion LanguageVersion;
    public string Condition;
    public List<string> Dependencies = new();
    public List<NodeBase> NodeDependencies = new();
    public List<string> Lines = new();
    public TempNodeLeaf(string name, NodeBase parent, Stream contentStream) : base(name, false, parent)
    {
        var sr = new StreamReader(contentStream);
        const int ReadDependencies = 0, ReadCondition = 1, ReadLine = 2;
        var state = 0;
        string? text = sr.ReadLine();
        LanguageVersion = Utils.GetLanguageVersion(text.Substring("//".Length).Trim());
        while (!sr.EndOfStream)
        {
            text = sr.ReadLine();
            if (string.IsNullOrEmpty(text))
            {
                continue;
            }
            switch (state)
            {
                case ReadDependencies:
                    if (text.StartsWith("//"))
                    {
                        Dependencies.Add(text.Substring("//".Length).Trim());
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
                    Lines.Add(text);
                    break;
                default:
                    throw new NotImplementedException("unknown state");
            }
        }
        if(text!.Trim() != "#endif")
        {
            throw new InvalidDataException("the last line must be '#endif'");
        }
        Lines.RemoveAt(Lines.Count - 1);
        if (string.IsNullOrEmpty(Condition))
        {
            throw new ArgumentException("Condition cannot be null");
        }
        // resolve null warn
        Condition ??= "";
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
    public NodeLeaf(string name, NodeBase parent, string condition, NodeLeaf[] dependencies, string[] lines) : base(name, false, parent ?? throw new ArgumentNullException(nameof(parent)))
    {
        Condition = condition;
        Dependencies = dependencies;
        Lines = lines;
    }
    public NodeLeaf CloneWithNewParent(NodeBase parent) => new(Name, parent, Condition, Dependencies, Lines) { Depth = Depth, LanguageVersion = LanguageVersion };
}