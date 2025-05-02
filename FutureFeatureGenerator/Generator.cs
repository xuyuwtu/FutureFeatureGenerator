using System.Collections.Immutable;
using System.IO;
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
    const string settingUseExtensions = "UseExtensions";
    const string settingUseRealCondition = "UseRealCondition";
    private readonly NodeRoot Root = new();
    private readonly Dictionary<NodeCommon, string> namespaceCache = new();
    private readonly List<NodeLeaf> allLeaf = new();
    private bool _isIniaialized = false;
#if UseIIncrementalGenerator
    public void Initialize(IncrementalGeneratorInitializationContext context)
#else
    public void Initialize(GeneratorInitializationContext context)
#endif
    {
#if UseIIncrementalGenerator
        context.RegisterSourceOutput(IncrementalValueProviderExtensions.Combine(context.AdditionalTextsProvider.Where(static text => string.Equals(FileName, Path.GetFileName(text.Path), StringComparison.OrdinalIgnoreCase)).Collect(), context.CompilationProvider.WithComparer(CompilationExternalReferencesEqualityComparer.Instance)), Execute);
#endif
        if (!_isIniaialized)
        {
            Init();
        }
    }
    private void Init()
    {
        _isIniaialized = true;
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
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            /*
            if is methodClass
                    #region
                    #endregion
                or
                    // <StartLine> <MethodName>
                    // ...
                end when not start with // (namespace ...)
            else
                namespace ...
                // <CSharpVersion>    
             */
            using var sr = new StreamReader(stream).GetWrapper();
            var text = sr.ReadLine();
            if (text.StartsWith("//"))
            {
                var parent = new NodeClass(names[count], node);
                node.AddChild(parent);
                var list = new List<(int startLine, string name)>();
                do
                {
                    var result = text.Substring("//".Length).Split(Utils.SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
                    if (result[0][result[0].Length - 1] == '+')
                    {
                        list.Add((-int.Parse(result[0].Remove(result[0].Length - 1)), result[1]));
                    }
                    else
                    {
                        list.Add((int.Parse(result[0]), result[1]));
                    }
                    text = sr.ReadLine();
                } while (text.StartsWith("//"));
                for (int i = 0; i < list.Count; i++)
                {
                    var (startLine, name) = list[i];
                    if (startLine < 0)
                    {
                        startLine = -startLine + list.Count - i - 1;
                    }
                    parent.AddChild(name, sr, 4, startLine);
                }
            }
            else if (text.StartsWith("#region"))
            {
                var parent = new NodeClass(names[count], node);
                node.AddChild(parent);
                while (!sr.EndOfStream)
                {
                    text = sr.ReadLine();
                    if (text.StartsWith("    #region"))
                    {
                        parent.AddChild(text.Substring("    #region".Length).Trim(), sr, 4, sr.CurrentLine);
                    }
                }
            }
            else
            {
                node.AddChild(names[count], sr, 0, 2);
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
                    if (inDegree.TryGetValue(dependency, out int value))
                    {
                        inDegree[dependency] = value + 1;
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
    public unsafe void Execute(GeneratorExecutionContext context)
#endif
    {
#if UseIIncrementalGenerator
        var additionalFiles = data.additionalFiles;
#else
        var additionalFiles = context.AdditionalFiles;
#endif
        if (additionalFiles.Length == 0)
        {
            return;
        }
#if UseIIncrementalGenerator
        var compilation = data.compilation;
#else
        var compilation = context.Compilation;
#endif
        if (compilation is not CSharpCompilation csharpCompilation)
        {
            return;
        }
        var compilationLanguageVersion = csharpCompilation.LanguageVersion;
        if (compilationLanguageVersion == LanguageVersion.Default)
        {
            compilationLanguageVersion = LanguageVersion.Latest;
        }
        var additionalText = additionalFiles.FirstOrDefault(x => string.Equals(FileName, Path.GetFileName(x.Path), StringComparison.OrdinalIgnoreCase));
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
        var additionalNodes = new List<NodeLeaf>();
        var depth = -1;
        var depthNode = new Stack<NodeCommon>();
        var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { settingUseExtensions, bool.FalseString },
            { settingUseRealCondition, bool.FalseString },
        };
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
            var lineSpan = line.AsSpan();
            if (Utils.SkipWhileSpaceFirstCharIs(lineSpan, commentChar))
            {
                continue;
            }
            if (line[0] == '@')
            {
                lineSpan = lineSpan.Slice(1);
                var tuples = lineSpan.Trim().Split(Utils.SpaceSeparator);
                if (tuples.Count >= 2)
                {
                    var settingName = lineSpan.Slice(tuples[0]);
                    foreach(var key in settings.Keys)
                    {
                        if(settingName.Equals(key.AsSpan(), StringComparison.OrdinalIgnoreCase))
                        {
                            settings[key] = lineSpan.Slice(tuples[1]).ToString();
                            break;
                        }
                    }
                }
                continue;
            }
            if(!TryGetDepth(lineSpan, out var newDepth))
                {
                continue;
            }
            if (depth + 1 != newDepth)
            {
                if (newDepth > depth)
                {
                    continue;
                }
                var targetCount = newDepth + 1;
                while (depthNode.Count > targetCount)
                {
                    depthNode.Pop();
                }
                depth = newDepth - 1;
            }
            var trimedLine = lineSpan.Trim();
            NodeBase? node = depthNode.Peek();
            if (node is null)
            {
                break;
            }
            var ranges = trimedLine.Split(char.IsWhiteSpace);
                string? modifer = null;
            if (ranges.Count > 1)
                {
                if (Array.IndexOf(Modifiers.All, trimedLine.Slice(ranges[1]).ToString()) is var modIndex && modIndex != -1)
                    {
                        modifer = Modifiers.All[modIndex];
                    }
                trimedLine = trimedLine.Slice(ranges[0]);
                }
            var nameList = trimedLine.Split(Utils.PointSeparator);
            var enumerator = nameList.GetEnumerator();
            NodeCommon? newDepthNode = null;
            while (enumerator.MoveNext())
                {
                var checkName = trimedLine.Slice(enumerator.Current).ToString();
                var nodes = node.FindAllNode(checkName);
                if (nodes.IsNullOrEmpty())
                    {
                    break;
                }
                foreach (var foundNode in nodes)
                {
                    if (foundNode is NodeLeaf nodeLeaf)
                {
                    additionalNodes.Add(nodeLeaf);
                    if (modifer is not null)
                    {
                        modifierCache[nodeLeaf] = modifer;
                    }
                    }
                }
                if (nodes.Count == 1)
                {
                    node = nodes[0];
                    newDepthNode = node as NodeCommon;
                    if (modifer is not null && newDepthNode is not null)
                {
                        if (newDepthNode.GetType() == typeof(NodeClass))
                    {
                        modifierCache[node] = modifer;
                    }
                    else
                    {
                            foreach (var children in newDepthNode.Children)
                        {
                            modifierCache[children] = modifer;
                        }
                    }
                }
            }
                else
                {
                    newDepthNode = null;
                    break;
                }
            }
            if (newDepthNode is not null)
            {
                depthNode.Push(newDepthNode);
            }
            depth++;
        }
        var useExtensions = settings[settingUseExtensions].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
        var useRealCondition = settings[settingUseRealCondition].Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
        var count = additionalNodes.Count;
        for (int i = 0; i < count; i++)
        {
            additionalNodes.AddRange(additionalNodes[i].Dependencies);
        }
        var groups = additionalNodes.Where(static x => x.Parent is not NodeClass).Distinct().GroupBy(x => namespaceCache[(NodeCommon)x.Parent!], static x => x, ReferenceEqualityComparer<string>.Instance);
        var isNodeClassNodes = additionalNodes.Where(static x => x.Parent is NodeClass).Distinct().ToArray();
        var namespaceTexts = new string[groups.Count()];
        var namespaceGroup = new List<NodeLeaf>[namespaceTexts.Length];
        var index = 0;
        foreach (var group in groups)
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
            namespaceHandles.AsSpan().Clear();
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
                    if (i == -1)
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
                    if (removeIndex != -1)
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
        var preprocessorSymbolNames = csharpCompilation.SyntaxTrees.FirstOrDefault()?.Options.PreprocessorSymbolNames.ToArray();
        if (preprocessorSymbolNames is { Length: > 0 })
        {
            foreach (var node in isNodeClassNodes)
            {
                if (node.ConditionFunc(preprocessorSymbolNames))
                {
                    additionalNodes.Add(node);
                }
            }
        }
        else
        {
            additionalNodes.AddRange(isNodeClassNodes);
        }
        foreach (var nodes in namespaceGroup)
        {
            additionalNodes.AddRange(nodes);
        }
        if (additionalNodes.Count == 0)
        {
            return;
        }
        additionalNodes = additionalNodes.Distinct(ReferenceEqualityComparer<NodeLeaf>.Instance).ToList();
        additionalNodes.Sort(static (x1, x2) => x1.Order.CompareTo(x2.Order));
        var itw = new StreamIndentedTextWriter(writeStringCache, memoryStream);
        itw.WriteLine("#nullable enable");
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
        var needWriteEndIf = false;
        var ifConditionString = condititonCache.GetOrAdd("true");
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
                        if (needWriteEndIf)
                        {
                            itw.WriteLine("#endif");
                        }
                        needWriteEndIf = false;
                    }
                    itw.CloseBrace();
                }
                continue;
            }
            if (enumerator.Current is NodeCommon nodeCommon)
            {
                if (!lastWritedEndIf)
                {
                    lastWritedEndIf = true;
                    if (needWriteEndIf)
                    {
                        itw.WriteLine("#endif");
                    }
                    needWriteEndIf = false;
                }
                itw.WriteLine(nodeCommon.GetText(modifierCache.TryGetValue(nodeCommon, out var modifer) ? modifer : defaultModifer));
                itw.OpenBrace();
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
                        if (needWriteEndIf)
                        {
                            itw.WriteLine("#endif");
                        }
                        needWriteEndIf = false;
                    }
                    if (!ReferenceEquals(ifConditionString, leaf.Condition))
                    {
                        needWriteEndIf = true;
                        itw.WriteLine($"#if {leaf.Condition}");
                    }
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

    private static bool TryGetDepth(ReadOnlySpan<char> line, out int depth)
    {
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
        if (spaceLength % 4 != 0)
        {
            depth = 0;
            return false;
        }
        depth = spaceLength / 4;
        return true;
    }
}
