using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace FutureFeatureGenerator;

enum NodeType
{
    Namespace,
    Class,
    Method
}
[DebuggerDisplay("Name={Name} Id={Id}")]
internal abstract class NodeBase
{
    public NodeType NodeType { get; set; }
    public string Name { get; set; }
    public string? AliasName { get; set; }
    public int ParentId { get; set; } = -1;
    public int Id { get; set; } = -1;
    public abstract bool IsLeaf { get; }
    public NodeBase(NodeType nodeType, string name)
    {
        NodeType = nodeType;
        Name = name;
    }
    public abstract void Write(IndentedTextWriter writer, Options options, Dictionary<int, string> modifierRecord, string[] nodesNamespace);
    public virtual void ThrowIfNotValid()
    {
        FutureArgumentOutOfRangeException.ThrowIfLessThan(Id, 0);
    }
    public IEnumerable<NodeBase> GetDependencies()
    {
        if(this is NodeClass nodeClass)
        {
            return nodeClass.Dependencies.Cast<NodeBase>();
        }
        if(this is NodeMethod nodeMethod)
        {
            return nodeMethod.Dependencies.Cast<NodeBase>();
        }
        return [];
    }
}
internal abstract class HasChildrenNode : NodeBase
{
    public List<NodeBase> Children { get; set; }
    protected HasChildrenNode(NodeType nodeType, string name) : base(nodeType, name)
    {
        Children = [];
    }
    public HasChildrenNode CloneWithoutChildren()
    {
        var result = (HasChildrenNode)MemberwiseClone();
        result.Children = [];
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
    public bool FindFullNameNode(string fullName, [MaybeNullWhen(false)] out NodeBase node)
    {
        node = null;
        string[] names = fullName.Contains('.') ? fullName.Split('.') : [fullName];
        HasChildrenNode? curNode = this;
        int i = 0;
        for (; i < names.Length; i++)
        {
            if(curNode is null)
            {
                node = null;
                break;
            }
            foreach (var child in curNode.Children)
            {
                if (child.Name == names[i])
                {
                    node = child;
                    curNode = child as HasChildrenNode;
                    break;
                }
            }
        }
        if(i != names.Length || node is null)
        {
            return false;
        }
        return true;
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
    public void AddChild(NodeBase node)
    {
        node.ThrowIfNotValid();
        ThrowIfExists(node.Name);
        node.ParentId = Id;
        Children.Add(node);
    }
    public NodeBase? FindNode(int targetId)
    {
        if(Id == targetId)
        {
            return this;
        }
        foreach (var child in Children)
        {
            if(child is HasChildrenNode childrenNode)
            {
                return childrenNode.FindNode(targetId);
            }
            if (child.Id == targetId)
            {
                return child;
            }
        }
        return null;
    }
    public IReadOnlyList<NodeBase>? FindAllNode(string name)
    {
        if (name == "*")
        {
            return Children;
        }
        List<NodeBase>? result = null;
        foreach (var child in Children)
        {
            if (string.Equals(name, child.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(name, child.AliasName, StringComparison.OrdinalIgnoreCase))
            {
                (result ??= []).Add(child);
            }
        }
        return result;
    }
}
internal class NodeNamespace : HasChildrenNode
{
    public override bool IsLeaf => false;
    public NodeNamespace(string name) : base(NodeType.Namespace, name)
    {
    }

    public override void Write(IndentedTextWriter writer, Options options, Dictionary<int, string> modifierRecord, string[] nodesNamespace)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            writer.Write("namespace ");
            writer.WriteLine(Name);
            writer.WriteLine('{');
            writer.Indent++;
        }
        foreach (var group in Children.OfType<NodeClass>().GroupBy(static x => x.Condition, ReferenceEqualityComparer<string>.Instance))
        {
            var writeIf = !string.IsNullOrWhiteSpace(group.Key);
            if (writeIf)
            {
                writer.Write("#if ");
                writer.WriteLine(group.Key);
            }
            foreach (var child in group)
            {
                child.Write(writer, options, modifierRecord, nodesNamespace);
            }
            if (writeIf)
            {
                writer.WriteLine("#endif");
            }
        }
        foreach (var child in Children.OfType<NodeNamespace>())
        {
            child.Write(writer, options, modifierRecord, nodesNamespace);
        }
        if (!string.IsNullOrEmpty(Name))
        {
            writer.Indent--;
            writer.WriteLine('}');
        }
    }
}
internal class NodeClass : HasChildrenNode
{
    public override bool IsLeaf => IsMaster;
    public bool IsMaster { get; set; }
    public string Condition { get; set; }
    public int ModifierLineIndex { get; set; } = -1;
    public string[]? Lines { get; set; }
    public object[]? Dependencies { get; set; } 
    public NodeClass(string condition, string name) : base(NodeType.Class, name)
    {
        if (!NodeMethod.CondititonFuncCache.TryGetValue(condition, out var func))
        {
            func = Utils.GetConditionFunc(condition);
            NodeMethod.CondititonFuncCache.Add(condition, func);
        }
        Condition = condition;
    }
    public override void Write(IndentedTextWriter writer, Options options, Dictionary<int, string> modifierRecord, string[] nodesNamespace)
    {
        var modifier = modifierRecord.TryGet(Id);
        if (modifier is null) 
        {
            modifier = modifierRecord.TryGet(ParentId);
            if(modifier is not null)
            {
                modifierRecord[Id] = modifier;
            }
        }
        modifier ??= Modifiers.Internal;
        if (IsMaster)
        {
            for (int i = 0; i < Lines!.Length; i++)
            {
                if (i == ModifierLineIndex)
                {
                    Utils.WriteGeneratedCodeAttribute(writer, nodesNamespace[Id]);
                    writer.Write(modifier);
                    writer.Write(' ');
                }
                writer.WriteLine(Lines[i]);
            }
        }
        else
        {
            Utils.WriteGeneratedCodeAttribute(writer, nodesNamespace[Id]);
            writer.Write(modifier);
            writer.Write(" static partial class Future");
            writer.WriteLine(Name);
            writer.WriteLine('{');
            writer.Indent++;
            var writeMethods = Children.Cast<NodeMethod>();
            if (options.UseExtensions)
            {
                var statices = writeMethods.Where(static x => x.IsStatic);
                if (statices.Any())
                {
                    writer.Write("extension(");
                    writer.Write(Name);
                    writer.WriteLine(')');
                    writer.WriteLine('{');
                    writer.Indent++;
                    foreach (var group in statices.GroupBy(x => x.GetCondition(options.UseRealCondition), ReferenceEqualityComparer<string>.Instance))
                    {
                        var writeIf = !string.IsNullOrWhiteSpace(group.Key);
                        if (writeIf)
                        {
                            writer.Write("#if ");
                            writer.WriteLine(group.Key);
                        }
                        foreach (var child in group)
                        {
                            child.Write(writer, options, modifierRecord, nodesNamespace);
                        }
                        if (writeIf)
                        {
                            writer.WriteLine("#endif");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine('}');

                    writeMethods = writeMethods.Where(static x => !x.IsStatic);
                }
            }
            foreach (var group in writeMethods.GroupBy(x => x.GetCondition(options.UseRealCondition), ReferenceEqualityComparer<string>.Instance))
            {
                var writeIf = !string.IsNullOrWhiteSpace(group.Key);
                if (writeIf)
                {
                    writer.Write("#if ");
                    writer.WriteLine(group.Key);
                }
                foreach (var child in group)
                {
                    child.Write(writer, options, modifierRecord, nodesNamespace);
                }
                if (writeIf)
                {
                    writer.WriteLine("#endif");
                }
            }
            writer.Indent--;
            writer.WriteLine('}');
        }
    }
    public override void ThrowIfNotValid()
    {
        base.ThrowIfNotValid();
        if (IsMaster)
        {
            FutureArgumentNullException.ThrowIfNull(Lines);
            FutureArgumentOutOfRangeException.ThrowIfLessThan(ModifierLineIndex, 0);
        }
        else
        {
            FutureArgumentNullException.ThrowIfNull(Children);
        }
    }
    public bool IsConditionTrue(string[] args)
    {
        return NodeMethod.CondititonFuncCache[Condition](args);
    }
}
internal class NodeMethod : NodeBase
{
    internal static readonly Dictionary<string, Func<string[], bool>> CondititonFuncCache = new(ReferenceEqualityComparer<string>.Instance);
    internal static string TrueCondition = null!;
    public override bool IsLeaf => true;
    public bool IsStatic { get; set; }
    public string Condition { get; set; }
    public string[] Lines { get; set; }
    public int ModifierLineIndex { get; set; } = -1;
    public object[]? Dependencies { get; set; }
    public NodeMethod(string[] lines, string condition, string name) : base(NodeType.Method, name)
    {
        Lines = lines;
        if(!CondititonFuncCache.TryGetValue(condition, out var func))
        {
            func = Utils.GetConditionFunc(condition);
            CondititonFuncCache.Add(condition, func);
        }
        Condition = condition;
    }

    public override void Write(IndentedTextWriter writer, Options options, Dictionary<int, string> modifierRecord, string[] nodesNamespace)
    {
        for (int i = 0; i < Lines.Length; i++)
        {
            if (i == ModifierLineIndex)
            {
                Utils.WriteGeneratedCodeAttribute(writer, nodesNamespace[Id]);
                writer.Write(modifierRecord.TryGet(Id) ?? modifierRecord.TryGet(ParentId) ?? Modifiers.Internal);
                writer.Write(' ');
            }
            writer.WriteLine(Lines[i]);
        }
    }
    public override void ThrowIfNotValid()
    {
        base.ThrowIfNotValid();
        FutureArgumentOutOfRangeException.ThrowIfLessThan(ModifierLineIndex, 0);
    }
    public string GetCondition(bool real)
    {
        if (IsStatic)
        {
            if (real)
            {
                return Condition;
            }
            return TrueCondition;
        }
        return Condition;
    }
    public bool IsConditionTrue(bool real, string[] args)
    {
        return CondititonFuncCache[GetCondition(real)](args);
    }
}