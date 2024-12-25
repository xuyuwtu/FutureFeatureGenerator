﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FutureFeatureGenerator;
[Generator(LanguageNames.CSharp)]
#if UseIIncrementalGenerator
public class IncrementalGenerator : IIncrementalGenerator
#else
public class SourceGenerator : ISourceGenerator
#endif
{
    private static readonly StringCache condititonCache = new();
    private const string FileName = "FutureFeature.txt";
    private readonly NodeRoot Root = new();
    private readonly Dictionary<NodeCommon, string> namespaceCache = new();
    private readonly List<NodeLeaf> allLeaf = new();
#if UseIIncrementalGenerator
    public void Initialize(IncrementalGeneratorInitializationContext context)
#else
    public void Initialize(GeneratorInitializationContext context)
#endif
    {
#if UseIIncrementalGenerator
        context.RegisterImplementationSourceOutput(IncrementalValueProviderExtensions.Combine(context.AdditionalTextsProvider.Collect(), context.CompilationProvider), Execute);
#endif
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.EndsWith(".cs"))
            {
                throw new InvalidDataException("not end with '.cs'");
            }
            var names = resourceName.Split(Utils.Separator);
            var count = names.Length - 2;
            NodeCommon node = Root;
            for (int i = 0; i < count; i++)
            {
                var name = names[i];
                if (node.FindNode(name, out var findNode))
                {
                    node = (NodeCommon)findNode;
                }
                else
                {
                    node = node.AddChild(name);
                }
            }
            node.AddChild(names[count], assembly.GetManifestResourceStream(resourceName)!);
        }
        var hasDependencyLeaf = new List<TempNodeLeaf>();
        var tempAllLeaf = new List<NodeBase>();
        var nodeQueue = new Queue<NodeBase>();
        nodeQueue.Enqueue(Root);
        while (nodeQueue.Count > 0)
        {
            var current = nodeQueue.Dequeue();
            if (current is TempNodeLeaf nodeLeaf)
            {
                if (nodeLeaf.Dependencies.Count == 0)
                {
                    var leaf = nodeLeaf.GetNodeLeaf(condititonCache, Array.Empty<NodeLeaf>());
                    var list = ((NodeCommon)nodeLeaf.Parent!).Children;
                    list[list.IndexOf(nodeLeaf)] = leaf;
                    tempAllLeaf.Add(leaf);
                }
                else
                {
                    hasDependencyLeaf.Add(nodeLeaf);
                }
            }
            else
            {
                foreach (var child in (NodeCommon)current)
                {
                    nodeQueue.Enqueue(child);
                }
            }
        }
        foreach (var nodeLeaf in hasDependencyLeaf)
        {
            tempAllLeaf.Add(nodeLeaf);
            foreach (var dependency in nodeLeaf.Dependencies)
            {
                nodeLeaf.NodeDependencies.Add(Root.FindNodeByFullName(dependency) ?? throw new InvalidDataException($"'{dependency}' not found"));
            }
        }
        tempAllLeaf.Sort(static (x1, x2) =>
        {
            if (x1 is NodeLeaf)
            {
                if (x2 is NodeLeaf)
                {
                    return 0;
                }
                return -1;
            }
            if (x1 is TempNodeLeaf)
            {
                if (x2 is TempNodeLeaf)
                {
                    return ((TempNodeLeaf)x1).Dependencies.Count.CompareTo(((TempNodeLeaf)x2).Dependencies.Count);
                }
                return 1;
            }
            return 0;
        });
        if (tempAllLeaf.Any(static x => x is TempNodeLeaf))
        {
            var inDegree = new Dictionary<NodeBase, int>();
            var queue = new Queue<NodeBase>();
            var sortedList = new List<NodeBase>();

            foreach (var node in tempAllLeaf)
            {
                inDegree[node] = 0;
            }

            foreach (var node in tempAllLeaf)
            {
                if (node is not TempNodeLeaf tempLeaf)
                {
                    continue;
                }
                foreach (var dependency in tempLeaf.NodeDependencies)
                {
                    if (inDegree.ContainsKey(dependency))
                    {
                        inDegree[dependency]++;
                    }
                    else
                    {
                        inDegree[dependency] = 1;
                    }
                }
            }

            foreach (var node in inDegree)
            {
                if (node.Value == 0)
                {
                    queue.Enqueue(node.Key);
                }
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                sortedList.Add(current);
                if (current is TempNodeLeaf nodeLeaf)
                {
                    foreach (var dependency in nodeLeaf.NodeDependencies)
                    {
                        inDegree[dependency]--;
                        if (inDegree[dependency] == 0)
                        {
                            queue.Enqueue(dependency);
                        }
                    }
                }
            }
            if (sortedList.Count != tempAllLeaf.Count)
            {
                throw new InvalidOperationException("Graph has cycles, topological sorting is not possible.");
            }
            for (int i = sortedList.Count - 1; i >= 0; i--)
            {
                if (sortedList[i] is TempNodeLeaf nodeLeaf)
                {
                    var dependencies = new NodeLeaf[nodeLeaf.Dependencies.Count];
                    for (int j = 0; j < dependencies.Length; j++)
                    {
                        dependencies[j] = Root.FindNodeByFullName(nodeLeaf.Dependencies[j]) as NodeLeaf ?? throw new InvalidDataException($"'{nodeLeaf.Dependencies[j]}' as NodeLeaf not found");
                    }
                    var list = ((NodeCommon)nodeLeaf.Parent!).Children;
                    list[list.IndexOf(nodeLeaf)] = nodeLeaf.GetNodeLeaf(condititonCache, dependencies);
                }
            }
        }
        var order = 0;
        var stack = new Stack<NodeBase>();
        stack.Push(Root);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.HasChildren)
            {
                var node = (NodeCommon)current;
                var names = new string[node.Depth];
                if (node.Depth != 0)
                {
                    var index = node.Depth;
                    var tempNode = node;
                    while (index-- > 0)
                    {
                        names[index] = tempNode!.Name;
                        tempNode = tempNode.Parent as NodeCommon;
                    }
                    namespaceCache.Add(node, string.Join(".", names));
                }
                node.Children.Sort(static (x1, x2) =>
                {
                    if (x1 is NodeCommon && x2 is NodeLeaf)
                    {
                        return -1;
                    }
                    if (x1 is NodeLeaf && x2 is NodeCommon)
                    {
                        return 1;
                    }
                    if (x1 is NodeLeaf && x2 is NodeLeaf)
                    {
                        return condititonCache.IndexOf(((NodeLeaf)x1).Condition).CompareTo(condititonCache.IndexOf(((NodeLeaf)x2).Condition));
                    }
                    return 0;
                });
                foreach (var child in node.Children)
                {
                    stack.Push(child);
                }
            }
            else
            {
                var leaf = (NodeLeaf)current;
                leaf.Order = order++;
                allLeaf.Add(leaf);
            }
        }
    }
#if UseIIncrementalGenerator
    private void Execute(SourceProductionContext context, (ImmutableArray<AdditionalText> additionalFiles, Compilation compilation) data)
#else
    public void Execute(GeneratorExecutionContext context)
#endif
    {
#if UseIIncrementalGenerator
        var compilation = data.compilation;
#else
        var compilation = context.Compilation;
#endif
        if (compilation is not CSharpCompilation csharpCompilation)
        {
            return;
        }
#if UseIIncrementalGenerator
        var additionalFiles = data.additionalFiles;
#else
        var additionalFiles = context.AdditionalFiles;
#endif
        var compilationLanguageVersion = csharpCompilation.LanguageVersion;
        if (compilationLanguageVersion == LanguageVersion.Default)
        {
            compilationLanguageVersion = LanguageVersion.Latest;
        }
        var additionalText = additionalFiles.FirstOrDefault(x => FileName.Equals(Path.GetFileName(x.Path), StringComparison.OrdinalIgnoreCase));
        if (additionalText is null)
        {
            return;
        }
        var lines = File.ReadAllLines(additionalText.Path);
        if (lines.Length == 0)
        {
            return;
        }
        var stringWriter = new StringWriter();
        var itw = new System.CodeDom.Compiler.IndentedTextWriter(stringWriter);
        var additionalNodes = new List<NodeLeaf>();
        var depth = -1;
        var depthNode = new Stack<NodeCommon>();
        var depthNodeCount = new Stack<int>();
        depthNode.Push(Root);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            if (i == 0 && line[0] == '*')
            {
                additionalNodes.AddRange(allLeaf);
                break;
            }
            if (line[0] == ';')
            {
                continue;
            }
            var spaceLength = 0;
            for (int j = 0; j < line.Length; j++)
            {
                if (line[j] is not ('\t' or ' '))
                {
                    break;
                }
                if (line[j] == '\t')
                {
                    spaceLength += 4;
                }
                else
                {
                    spaceLength++;
                }
            }
            if (spaceLength % 4 != 0)
            {
                continue;
            }
            var newDepth = spaceLength / 4;
            if (depth + 1 != newDepth)
            {
                if (newDepth > depth)
                {
                    continue;
                }
                var targetCount = newDepth + 1;
                while (depthNodeCount.Count > targetCount)
                {
                    depthNodeCount.Pop();
                }
                var depthCount = depthNodeCount.Pop();
                while (depthNode.Count > depthCount)
                {
                    depthNode.Pop();
                }
                depth = newDepth - 1;
            }
            depthNodeCount.Push(depthNode.Count);
            line = line.Trim();
            var nameList = line.Split(Utils.Separator, StringSplitOptions.RemoveEmptyEntries);
            NodeBase? node = depthNode.Peek();
            if (node is null)
            {
                break;
            }
            var enumerator = ((IEnumerable<string>)nameList).GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == "*")
                {
                    additionalNodes.AddRange(((NodeCommon)node).Children.OfType<NodeLeaf>());
                    break;
                }
                node = node.FindNode(enumerator.Current);
                if (node is null)
                {
                    break;
                }
                if (node is NodeLeaf)
                {
                    additionalNodes.Add((NodeLeaf)node);
                    break;
                }
                depthNode.Push((NodeCommon)node);
            }
            depth++;
        }
        var removeCount = 0;
        var count = additionalNodes.Count;
        for (int i = 0; i < count; i++)
        {
            var idx = i - removeCount;
            if (additionalNodes[idx].LanguageVersion > csharpCompilation.LanguageVersion)
            {
                additionalNodes.RemoveAt(idx);
                removeCount++;
            }
        }
        var groups = additionalNodes.Distinct().GroupBy(x => namespaceCache[(NodeCommon)x.Parent!], static x => x, ReferenceEqualityComparer<string>.Instance);
        var namespaceTexts = new string[groups.Count()];
        var namespaceGroup = new List<NodeLeaf>[namespaceTexts.Length];
        var index = 0;
        foreach(var group in groups)
        {
            namespaceTexts[index] = group.Key;
            namespaceGroup[index] = group.ToList();
            index++;
        }
        var namespaceHandles = new StringHandle[namespaceTexts.Length];
        var notNodeTypeNamespaceStringHandle = new List<StringHandle>();
        var checkReference = true;
        foreach (var reference in compilation.ExternalReferences)
        {
            if (!checkReference)
            {
                break;
            }
            if (reference is not PortableExecutableReference executableReference || executableReference.FilePath is null || executableReference.GetMetadata().Kind != MetadataImageKind.Assembly)
            {
                continue;
            }
            namespaceHandles.AsSpan().Fill(default);
            notNodeTypeNamespaceStringHandle.Clear();
            try
            {
                using var peStream = File.OpenRead(executableReference.FilePath);
                var peReader = new PEReader(peStream);
                if (!peReader.HasMetadata)
                {
                    continue;
                }
                var mtReader = peReader.GetMetadataReader();
                foreach (var typeDefinitionHandle in mtReader.TypeDefinitions)
                {
                    var typeDefinition = mtReader.GetTypeDefinition(typeDefinitionHandle);
                    if ((typeDefinition.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
                    {
                        continue;
                    }
                    if (notNodeTypeNamespaceStringHandle.Contains(typeDefinition.Namespace))
                    {
                        continue;
                    }
                    var i = Array.IndexOf(namespaceHandles, typeDefinition.Namespace);
                    if(i == -1)
                    {
                        var nameSpace = mtReader.GetString(typeDefinition.Namespace);
                        i = Array.IndexOf(namespaceTexts, nameSpace);
                        if (i == -1)
                        {
                            notNodeTypeNamespaceStringHandle.Add(typeDefinition.Namespace);
                            continue;
                        }
                        namespaceHandles[i] = typeDefinition.Namespace;
                    }
                    var name = mtReader.GetString(typeDefinition.Name);
                    int removeIndex = -1;
                    var group = namespaceGroup[i];
                    for (int j = 0; j < group.Count; j++)
                    {
                        var node = group[j];
                        if (string.Equals(name, node.Name, StringComparison.Ordinal))
                        {
                            removeIndex = j;
                            break;
                        }
                    }
                    if(removeIndex != -1)
                    {
                        group.RemoveAt(removeIndex);
                    }
                }
            }
            catch
            {
                checkReference = false;
            }
        }
        additionalNodes.Clear();
        foreach(var nodes in namespaceGroup)
        {
            additionalNodes.AddRange(nodes);
        }
        if (additionalNodes.Count == 0)
        {
            return;
        }
        count = additionalNodes.Count;
        for (int i = 0; i < count; i++)
        {
            additionalNodes.AddRange(additionalNodes[i].Dependencies);
        }
        additionalNodes = additionalNodes.Distinct().ToList();
        additionalNodes.Sort(static (x1, x2) => x1.Order.CompareTo(x2.Order));
        foreach (var additionalNode in additionalNodes)
        {
            itw.WriteLine($"// {namespaceCache[(NodeCommon)additionalNode.Parent!]}.{additionalNode.Name}");
        }
        var newRoot = new NodeRoot();
        var addStack = new Stack<NodeBase>();
        for (int i = 0; i < additionalNodes.Count; i++)
        {
            NodeBase? node = additionalNodes[i];
            while (node.Parent is not null)
            {
                addStack.Push(node.Parent);
                node = node.Parent;
            }
            NodeCommon node2 = newRoot;
            while (addStack.Count > 0)
            {
                node2 = node2.GetOrAddChild(addStack.Pop().Name);
            }
            node2.Children.Add(additionalNodes[i].CloneWithNewParent(node2));
        }
        var enumerators = new List<NodeBase>.Enumerator[additionalNodes.Select(x => x.Depth).Max()];
        index = 0;
        enumerators[0] = newRoot.GetEnumerator();
        NodeLeaf? lastLeaf = null;
        var lastWritedEndIf = true;
        while (index != -1)
        {
            ref var enumerator = ref enumerators[index];
            if (!enumerator.MoveNext())
            {
                index--;
                if (index != -1)
                {
                    if (!lastWritedEndIf)
                    {
                        lastWritedEndIf = true;
                        itw.WriteLine("#endif");
                    }
                    itw.Indent--;
                    itw.WriteLine('}');
                }
                continue;
            }
            if (enumerator.Current is NodeCommon nodeCommon)
            {
                if (!lastWritedEndIf)
                {
                    lastWritedEndIf = true;
                    itw.WriteLine("#endif");
                }
                itw.WriteLine($"namespace {nodeCommon.Name}");
                itw.WriteLine('{');
                itw.Indent++;
                index++;
                enumerators[index] = nodeCommon.Children.GetEnumerator();
            }
            else
            {
                var leaf = (NodeLeaf)enumerator.Current;
                if (lastLeaf is null || !ReferenceEquals(lastLeaf.Parent, leaf.Parent) || !ReferenceEquals(lastLeaf.Condition, leaf.Condition))
                {
                    if (!lastWritedEndIf && lastLeaf is not null)
                    {
                        lastWritedEndIf = true;
                        itw.WriteLine("#endif");
                    }
                    itw.WriteLine($"#if {leaf.Condition}");
                }
                foreach (var line in leaf.Lines)
                {
                    itw.WriteLine(line);
                }
                lastLeaf = leaf;
                lastWritedEndIf = false;
            }
        }
        var result = stringWriter.ToString();
        Console.WriteLine(result);
        context.AddSource($"{nameof(FutureFeatureGenerator)}.cs", result);
    }
}