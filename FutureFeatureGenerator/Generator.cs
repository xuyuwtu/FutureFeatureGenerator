using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

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
    private static readonly StringCache conditionCache = new();
    private readonly Dictionary<int, string> modifierCache = [];
    private static readonly Regex requireTypeMatcher = new(@$"\[{nameof(RequireType)}\(nameof\((.*?)\)\)\]");
    public const string FileName = "FutureFeature.txt";
    public const string Version = "1.4.0";
    const char commentChar = ';';
    const char childrenLeafAllMatchChar = '*';
    const string childrenLeafAllMatchString = "*";
    private readonly NodeNamespace Root = new("");
    private NodeBase[] AllNodes = [];
    private NodeBase[] AllLeafNodes = [];
    private string[] NodesFullName = [];
    internal string[] NodesNamespace = [];
    private bool _isInitialized = false;
    static FeatureGenerator()
    {
        NodeMethod.TrueCondition = conditionCache.GetOrAdd("true");
    }
#if UseIIncrementalGenerator
    public void Initialize(IncrementalGeneratorInitializationContext context)
#else
    public void Initialize(GeneratorInitializationContext context)
#endif
    {
#if UseIIncrementalGenerator
        context.RegisterSourceOutput(context.AdditionalTextsProvider.Where(static text => string.Equals(FileName, Path.GetFileName(text.Path), StringComparison.OrdinalIgnoreCase)).Collect().Combine(context.CompilationProvider), Execute);
#endif
        if (!Volatile.Read(ref _isInitialized))
        {
            Init();
        }
    }
    private void Init()
    {
        _isInitialized = true;
        var assembly = Assembly.GetExecutingAssembly();
        int id = 0;
        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.EndsWith(".cs"))
            {
                continue;
            }
            var names = resourceName.Split(Utils.PointSeparator);
            var count = names.Length - 2;
            HasChildrenNode node = Root;
            for (int i = 0; i < count; i++)
            {
                var name = names[i];
                if (node.FindNode(name, out var findNode))
                {
                    node = (HasChildrenNode)findNode;
                }
                else
                {
                    var addNode = new NodeNamespace(name)
                    {
                        Id = id++
                    };
                    addNode.ThrowIfNotValid();
                    node.AddChild(addNode);
                    node = addNode;
                }
            }
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            /*
            if is methodClass
                #region
                #endregion
            else
                namespace ...
             */
            using var sr = new StreamReader(stream).GetWrapper();
            var text = sr.ReadLine();
            if (text.StartsWith("namespace"))
            {
                text = sr.ReadLine();
                Match match;
                var deps = new List<object>();
                while ((match = requireTypeMatcher.Match(text)).Success)
                {
                    deps.Add(match.Groups[1].Value);
                    text = sr.ReadLine();
                }
                if (!text.StartsWith("#if "))
                {
                    throw new InvalidDataException($"{resourceName} line:{sr.CurrentLine} must StartWith '#if '");
                }
                var condition = conditionCache.GetOrAdd(text.Substring("#if ".Length).Trim());
                var lines = new List<string>();
                var modifierIndex = -1;
                while (!sr.EndOfStream)
                {
                    text = sr.ReadLine();
                    if (text.StartsWith(Modifiers.Internal))
                    {
                        lines.Add(text.Substring(Modifiers.Internal.Length + 1));
                        modifierIndex = lines.Count - 1;
                    }
                    else
                    {
                        lines.Add(text);
                    }
                }
                while (true)
                {
                    if (string.IsNullOrEmpty(lines[lines.Count - 1]))
                    {
                        lines.RemoveAt(lines.Count - 1);
                    }
                    if (lines[lines.Count - 1].StartsWith("#endif"))
                    {
                        lines.RemoveAt(lines.Count - 1);
                        break;
                    }
                }
                var nodeClass = new NodeClass(condition, names[count])
                {
                    ModifierLineIndex = modifierIndex,
                    Dependencies = [.. deps],
                    IsMaster = true,
                    Lines = [.. lines],
                    Id = id++
                };
                node.AddChild(nodeClass);
            }
            else if (text.StartsWith("#region"))
            {
                var nodeClass = new NodeClass(conditionCache.GetOrAdd(""), names[count])
                {
                    Id = id++
                };
                node.AddChild(nodeClass);
                while (!sr.EndOfStream)
                {
                    text = sr.ReadLine();
                    //if(char.IsWhiteSpace(text[0]) && text.Substring(text.IndexOf('#'), "#region".Length) == "#region")
                    if (text.StartsWith("    #region"))
                    {
                        var nodeMethod = Utils.GetMethodNode(text.Substring("    #region".Length).Trim(), sr, 4, sr.CurrentLine);
                        nodeMethod.Condition = conditionCache.GetOrAdd(nodeMethod.Condition);
                        nodeMethod.Id = id++;
                        nodeClass.AddChild(nodeMethod);
                    }
                }
            }
            else
            {
                throw new Exception();
            }
        }
        var stack = new Stack<NodeBase>();
        stack.Push(Root);
        var allNodes = new List<NodeBase>();
        while(stack.Count > 0)
        {
            var node = stack.Pop();
            allNodes.Add(node);
            object[]? dependencies = null;
            if (node is NodeClass nodeClass)
            {
                dependencies = nodeClass.Dependencies;
            }
            else if (node is NodeMethod nodeMethod)
            {
                dependencies = nodeMethod.Dependencies;
            }
            if (dependencies is not null)
            {
                for (int i = 0; i < dependencies.Length; i++)
                {
                    var dep = (string)dependencies[i];
                    if (Root.FindFullNameNode(dep, out var depNode))
                    {
                        dependencies[i] = depNode;
                    }
                    else
                    {
                        throw new Exception($"'{dep}' not found in tree");
                    }
                }
            }
            if (node is HasChildrenNode namedNode)
            {
                foreach (var child in namedNode.Children)
                {
                    stack.Push(child);
                }
            }
        }
        // remove Root
        allNodes.RemoveAt(0);
        allNodes.Sort(static (x, y) => x.Id.CompareTo(y.Id));
        AllNodes = [.. allNodes];
        AllLeafNodes = [.. allNodes.Where(x => x.IsLeaf)];
        NodesFullName = new string[AllNodes.Length];
        NodesNamespace = new string[AllNodes.Length];
        for (int i = 0; i < AllNodes.Length; i++)
        {
            var node = AllNodes[i];
            NodesFullName[i] = node.ParentId == -1 ? node.Name : NodesFullName[node.ParentId] + "." + node.Name;
            NodesNamespace[i] = node.NodeType switch
            {
                NodeType.Namespace => NodesFullName[i],
                NodeType.Class or NodeType.Method => NodesNamespace[node.ParentId],
                _ => throw new InvalidEnumArgumentException($"NodeType: {node.NodeType}")
            };

        }
        foreach (var node in AllLeafNodes)
        {
            switch (node)
            {
                case NodeClass nodeClass:
                    if (!conditionCache.Exists(nodeClass.Condition))
                    {
                        throw new Exception($"condition '{nodeClass.Condition}' not at conditionCache");
                    }
                    break;
                case NodeMethod nodeMethod:
                    if (!conditionCache.Exists(nodeMethod.Condition))
                    {
                        throw new Exception($"condition '{nodeMethod.Condition}' not at conditionCache");
                    }
                    break;
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
        var additionalNodes = new List<NodeBase>();
        var additionalNodesFromAll = false;
        var depth = -1;
        var depthNode = new Stack<HasChildrenNode>();
        var options = new Options();
        depthNode.Push(Root);
        modifierCache.Clear();
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
                options.ExecuteChange(lineSpan.Slice(1));
                continue;
            }
            if (line[0] == '*')
            {
                additionalNodes.Clear();
                additionalNodes.AddRange(AllLeafNodes);
                additionalNodesFromAll = true;
                break;
            }
            if (!TryGetDepth(lineSpan, out var newDepth))
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
            var trimmedLine = lineSpan.Trim();
            NodeBase? node = depthNode.Peek();
            if (node is null)
            {
                break;
            }
            var ranges = trimmedLine.Split(char.IsWhiteSpace);
            string? modifier = null;
            if (ranges.Count > 1)
            {
                if (Array.IndexOf(Modifiers.All, trimmedLine.Slice(ranges[1]).ToString()) is var modIndex && modIndex != -1)
                {
                    modifier = Modifiers.All[modIndex];
                }
                trimmedLine = trimmedLine.Slice(ranges[0]);
            }
            var nameList = trimmedLine.Split(Utils.PointSeparator);
            if(newDepth == 0 && nameList.Count == 1)
            {
                var checkName = trimmedLine.Slice(nameList[0]).ToString();
                var nodes = AllLeafNodes.Where(x => string.Equals(checkName, x.Name) || string.Equals(checkName, x.AliasName));
                if (nodes.Any())
                {
                    additionalNodes.AddRange(nodes);
                    continue;
                }
            }
            var enumerator = nameList.GetEnumerator();
            HasChildrenNode? newDepthNode = null;
            while (enumerator.MoveNext())
            {
                var checkName = trimmedLine.Slice(enumerator.Current).ToString();
                var nodes = (node as HasChildrenNode)?.FindAllNode(checkName);
                if (nodes.IsNullOrEmpty())
                {
                    break;
                }
                if (nodes.Count == 1)
                {
                    node = nodes[0];
                    newDepthNode = node as HasChildrenNode;
                    if (modifier is not null)
                    {
                        modifierCache[node.Id] = modifier;
                    }
                }
                else
                {
                    newDepthNode = null;
                }
                foreach (var foundNode in nodes)
                {
                    if (foundNode.IsLeaf)
                    {
                        additionalNodes.Add(foundNode);
                    }
                }
                if(newDepthNode is null)
                {
                    break;
                }
            }
            if (newDepthNode is not null)
            {
                depthNode.Push(newDepthNode);
            }
            depth++;
        }
        if (!additionalNodesFromAll && !options.DisableAddDependencies)
        {
            var count = additionalNodes.Count;
            for (int i = 0; i < count; i++)
            {
                additionalNodes.AddRange(additionalNodes[i].GetDependencies());
            }
            additionalNodes = additionalNodes.Distinct(ReferenceEqualityComparer<NodeBase>.Instance).ToList();
        }
        var preprocessorSymbolNames = csharpCompilation.SyntaxTrees.FirstOrDefault()?.Options.PreprocessorSymbolNames.ToArray();
        if (options.UseRealCondition && preprocessorSymbolNames is { Length: > 0 })
        {
            var count = additionalNodes.Count;
            var offset = 0;
            for (int i = 0; i < count; i++)
            {
                var index = i + offset;
                var node = additionalNodes[index];
                if(node is NodeMethod nodeMethod && !nodeMethod.IsConditionTrue(options.UseRealCondition, preprocessorSymbolNames))
                {
                    additionalNodes.RemoveAt(index);
                    offset--;
                }
                else if(node is NodeClass nodeClass && !nodeClass.IsConditionTrue(preprocessorSymbolNames))
                {
                    additionalNodes.RemoveAt(index);
                    offset--;
                }
            }
        }
        if (additionalNodes.Count == 0)
        {
            return;
        }
        //var itw = new StreamIndentedTextWriter(writeStringCache, memoryStream);
        var memoryStream = new MemoryStream();
        var itw = new IndentedTextWriter(new StreamWriter(memoryStream) { AutoFlush = true });
        itw.WriteLine("#nullable enable");
        foreach (var additionalNode in additionalNodes)
        {
            itw.Write("// ");
            itw.WriteLine(NodesFullName[additionalNode.Id]);
        }
        BuildTree(additionalNodes, AllNodes).Write(itw, options, modifierCache, NodesNamespace);
        memoryStream.Position = 0;
        var result = new StreamReader(memoryStream).ReadToEnd();
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
    private static NodeNamespace BuildTree(List<NodeBase> nodes, NodeBase[] allNodes)
    {
        var result = new NodeNamespace("");
        var needIds = new Stack<int>();
        var thisAllNodes = new NodeBase[allNodes.Length];
        foreach(var addNode in nodes)
        {
            needIds.Clear();
            needIds.Push(addNode.ParentId);
            NodeBase? findNode;
            int findId;
            while (true)
            {
                findId = needIds.Peek();
                if (thisAllNodes[findId] is not null)
                {
                    findNode = thisAllNodes[findId];
                    needIds.Pop();
                    break;
                }
                var parentId = allNodes[findId].ParentId;
                if(parentId == -1)
                {
                    findNode = result;
                    break;
                }
                needIds.Push(parentId);
            }
            while (needIds.Count > 0)
            {
                findId = needIds.Pop();
                var parentNode = ((HasChildrenNode)allNodes[findId]).CloneWithoutChildren();
                thisAllNodes[findId] = parentNode;
                ((HasChildrenNode)findNode).AddChild(parentNode);
                findNode = parentNode;
            }
            ((HasChildrenNode)findNode!).AddChild(addNode);
        }
        return result;
    }
}
