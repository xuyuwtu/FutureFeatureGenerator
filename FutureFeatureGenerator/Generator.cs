using System.Collections.Generic;
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
public class FeatureGenerator : 
#if UseIIncrementalGenerator
    IIncrementalGenerator
#else
    ISourceGenerator
#endif
{
    private static readonly StringCache condititonCache = new();
    private static readonly StringCache writeStringCache = new();
    private readonly Dictionary<NodeBase, string> modifierCache = new(new NodeEqualityComparer());
    public const string FileName = "FutureFeature.txt";
    const char commentChar = ';';
    const char childrenLeafAllMatchChar = '*';
    const string childrenLeafAllMatchString = "*";
    private readonly NodeRoot Root = new();
    private readonly Dictionary<NodeCommon, string> namespaceCache = new();
    private readonly List<NodeLeaf> allLeaf = new();
    private readonly string[] methodTypeFile = new string[] 
    { 
        $"System.IO.{nameof(Stream)}.cs",
        $"System.{nameof(ArgumentException)}.cs",
        $"System.{nameof(ArgumentNullException)}.cs",
        $"System.{nameof(ArgumentOutOfRangeException)}.cs",
    };
#if UseIIncrementalGenerator
    public void Initialize(IncrementalGeneratorInitializationContext context)
#else
    public void Initialize(GeneratorInitializationContext context)
#endif
    {
#if UseIIncrementalGenerator
        context.RegisterSourceOutput(IncrementalValueProviderExtensions.Combine(context.AdditionalTextsProvider.Where(static text => string.Equals(FileName, Path.GetFileName(text.Path), StringComparison.OrdinalIgnoreCase)).Collect(), context.CompilationProvider.WithComparer(CompilationExternalReferencesEqualityComparer.Instance)
            ), Execute);
#endif
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.EndsWith(".cs"))
            {
                continue;
            }
            var names = resourceName.Split(Utils.PointSeparator);
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
            if (methodTypeFile.Contains(resourceName))
            {
                var parent = new NodeClass(names[count], node);
                node.AddChild(parent);
                using var stream = assembly.GetManifestResourceStream(resourceName)!;
                // format:
                // // <StartLine> <MethodName>
                // // ...
                var sr = new StreamReader(stream).GetWrapper();
                var list = new List<(int startLine, string name)>();
                var text = sr.ReadLine();
                do
                {
                    var result = text.Substring("//".Length).Split(Utils.SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                    list.Add((int.Parse(result[0]), result[1]));
                    text = sr.ReadLine();
                } while (text.StartsWith("//"));
                foreach (var pair in list)
                {
                    parent.AddChild(pair.name, sr, pair.startLine);
                }
            }
            else
            {
                using var stream = assembly.GetManifestResourceStream(resourceName)!;
                node.AddChild(names[count], stream);
            }
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
        // [NodeLeaf, ..., TempNodeLeaf, ...]
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
                // [NodeCommon, ..., NodeLeaf, ...]
                // if condition is:
                // NodeLeaf1 true
                // NodeLeaf2 false
                // NodeLeaf3 true
                // => [NodeLeaf1 { Order = 0 }, NodeLeaf3 { Order = 1 }, NodeLeaf2 { Order = 2 }]
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
                var isChecked = false;
                for (int i = 0; i < leaf.Lines.Length; i++)
                {
                    if (!isChecked && leaf.Lines[i].StartsWith(Modifiers.Internal))
                    {
                        leaf.Lines[i] = leaf.Lines[i].Substring(Modifiers.Internal.Length).Trim();
                        leaf.ModiferLineIndex = i;
                        isChecked = true;
                    }
                    leaf.Lines[i] = writeStringCache.GetOrAdd(leaf.Lines[i]);
                }
            }
        }
    }
#if UseIIncrementalGenerator
    private unsafe void Execute(SourceProductionContext context, (ImmutableArray<AdditionalText> additionalFiles, Compilation compilation) data)
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
        var additionalText = additionalFiles
#if UseIIncrementalGenerator
            .First();
#else
            .FirstOrDefault(x => string.Equals(FileName, Path.GetFileName(x.Path), StringComparison.OrdinalIgnoreCase));
#endif
        if (additionalText is null)
        {
            return;
        }
#if DEBUG
        string[] lines;
        var sourceText = additionalText.GetText(default);
        if (sourceText is null)
        {
            lines = File.ReadAllLines(additionalText.Path);
        }
        else 
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sourceText.Write(sw);
            sw.Flush();
            var tempLines = new List<string>();
            ms.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(ms);
            while (!sr.EndOfStream)
            {
                tempLines.Add(sr.ReadLine());
            }
            lines = tempLines.ToArray();
        }
#else
        var lines = File.ReadAllLines(additionalText.Path);
#endif
        if (lines.Length == 0)
        {
            return;
        }
        var memoryStream = new MemoryStream();
        var itw = new StreamIndentedTextWriter(writeStringCache, memoryStream);
        var additionalNodes = new List<NodeLeaf>();
        var depth = -1;
        var depthNode = new Stack<NodeCommon>();
        var depthNodeCount = new Stack<int>();
        depthNode.Push(Root);
        modifierCache.Clear();
        var defaultModifer = Modifiers.Internal;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            if (i == 0 && line[0] == childrenLeafAllMatchChar)
            {
                additionalNodes.AddRange(allLeaf);
                if (line.Contains(' ') && Array.IndexOf(Modifiers.All, line.Substring(line.IndexOf(' ')).Trim()) is var modIndex && modIndex != -1)
                {
                    defaultModifer = Modifiers.All[modIndex];
                }
                break;
            }
            if (line[0] == commentChar)
            {
                continue;
            }
            var spaceLength = 0;
            int j = 0;
            for (; j < line.Length; j++)
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
            if (spaceLength % 4 != 0 || line[j] == commentChar)
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
            var nameList = line.Split(Utils.PointSeparator, StringSplitOptions.RemoveEmptyEntries);
            NodeBase? node = depthNode.Peek();
            if (node is null)
            {
                break;
            }
            var enumerator = ((IEnumerable<string>)nameList).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var spaceIndex = enumerator.Current.IndexOf(' ');
                var checkName = enumerator.Current;
                string? modifer = null;
                if (spaceIndex != -1)
                {
                    if (Array.IndexOf(Modifiers.All, line.Substring(line.IndexOf(' ')).Trim()) is var modIndex && modIndex != -1)
                    {
                        modifer = Modifiers.All[modIndex];
                    }
                    checkName = checkName.Substring(0, spaceIndex);
                }
                if (checkName == childrenLeafAllMatchString)
                {
                    foreach (var nodeLeaf2 in ((NodeCommon)node).Children.OfType<NodeLeaf>())
                    {
                        additionalNodes.Add(nodeLeaf2);
                        if (modifer is not null)
                        {
                            modifierCache[nodeLeaf2] = modifer;
                        }
                    }
                    break;
                }
                node = node.FindNode(checkName);
                if (node is null)
                {
                    break;
                }
                if (node is NodeLeaf nodeLeaf)
                {
                    additionalNodes.Add(nodeLeaf);
                    if (modifer is not null)
                    {
                        modifierCache[nodeLeaf] = modifer;
                    }
                    break;
                }
                if (modifer is not null)
                {
                    if (node.GetType() == typeof(NodeClass))
                    {
                        modifierCache[node] = modifer;
                    }
                    else
                    {
                        var common = (NodeCommon)node;
                        foreach (var children in common.Children)
                        {
                            modifierCache[children] = modifer;
                        }
                    }
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
        foreach (MetadataReference reference in compilation.ExternalReferences)
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
                    var blobReader = mtReader.GetBlobReader(typeDefinition.Name);
                    var name = new Span<byte>(blobReader.StartPointer, blobReader.Length);
                    int removeIndex = -1;
                    var groupNodes = namespaceGroup[i];
                    for (int j = 0; j < groupNodes.Count; j++)
                    {
                        var node = groupNodes[j];
                        if (name.SequenceEqual(node.NameData))
                        {
                            removeIndex = j;
                            break;
                        }
                    }
                    if(removeIndex != -1)
                    {
                        groupNodes.RemoveAt(removeIndex);
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
                node2 = node2.GetOrAddChild((NodeCommon)addStack.Pop());
            }
            node2.Children.Add(additionalNodes[i].CloneWithNewParent(node2));
        }
        var enumerators = new List<NodeBase>.Enumerator[additionalNodes.Select(static x => x.Depth).Max()];
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
                itw.WriteLine(nodeCommon.GetText(modifierCache.TryGetValue(nodeCommon, out var modifer) ? modifer : defaultModifer));
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
                for (int i = 0; i < leaf.Lines.Length; i++)
                {
                    if (i == leaf.ModiferLineIndex)
                    {
                        itw.WriteLineWithSpace(modifierCache.TryGetValue(leaf, out var modifer) ? modifer : defaultModifer, leaf.Lines[i]);
                    }
                    else
                    {
                        itw.WriteLine(leaf.Lines[i]);
                    }
                }
                lastLeaf = leaf;
                lastWritedEndIf = false;
            }
        }
        var result = itw.ToString();
        context.AddSource($"{nameof(FutureFeatureGenerator)}.g.cs", result);
    }
}
