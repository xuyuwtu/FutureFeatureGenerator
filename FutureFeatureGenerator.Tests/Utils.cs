using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace FutureFeatureGenerator.Tests;

static class Utils
{
    public static string[] GetGeneratorTypeFullNames(string fileText, ReferenceAssemblies assemblies, LanguageVersion languageVersion = LanguageVersion.Default)
    {
        return GetGeneratorTypeFullNames(fileText, assemblies.ResolveAsync(null, default).Result, languageVersion);
    }
    public static string[] GetGeneratorTypeFullNames(string fileText, ImmutableArray<MetadataReference> references, LanguageVersion languageVersion = LanguageVersion.Default)
    {
        var compilation = CSharpCompilation.Create("Test", [CSharpSyntaxTree.ParseText("namespace { class TestClass { } }", new CSharpParseOptions(languageVersion))], references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var csharpGeneratorDriver = CSharpGeneratorDriver.Create(new FeatureGenerator());
        var generatorDriver = csharpGeneratorDriver.AddAdditionalTexts([MyText.From(fileText, FeatureGenerator.FileName)]);
        generatorDriver = generatorDriver.RunGenerators(compilation);
        var runResult = generatorDriver.GetRunResult();
        Assert.Single(runResult.Results);
        var result = runResult.Results[0];
        Assert.False(result.GeneratedSources.Length > 1);
        if (result.GeneratedSources.Length == 0)
        {
            return Array.Empty<string>();
        }
        var tree = result.GeneratedSources[0].SyntaxTree;
        var array = tree.GetRoot().ChildNodes().First().GetLeadingTrivia().ToArray();
        var count = array.Length / 2;
        var names = new string[count];
        var text = tree.GetText();
        for (int i = 0; i < count; i++)
        {
            var span = array[i * 2].Span;
            var index = -1;
            for(int j = span.End; j >= span.Start; j--)
            {
                if (text[j] == '.')
                {
                    index = j + 1;
                    break;
                }
            }
            names[i] = text.ToString(TextSpan.FromBounds(index, span.End));
        }
        return names;
    }
    public static bool PerfectMatch(string[] fullNames, params string[] names)
    {
        if (fullNames.Length != names.Length)
        {
            return false;
        }
        for (int i = 0; i < fullNames.Length; i++)
        {
            if (!string.Equals(names[i], fullNames[i], StringComparison.Ordinal))
            {
                return false;
            }
        }
        return true;
    }
    public static bool AllMatch(string[] fullNames, params string[] names)
    {
        if(fullNames.Length != names.Length)
        {
            return false;
        }
        for(int i = 0; i < fullNames.Length; i++)
        {
            if(!names.Contains(fullNames[i], StringComparer.Ordinal))
            {
                return false;
            }
        }
        return true;
    }
}