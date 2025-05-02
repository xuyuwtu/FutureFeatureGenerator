using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace FutureFeatureGenerator.Console.Test;

internal class Program
{
    static void Main(string[] args)
    {
        var compilation = CSharpCompilation.Create("Test", [CSharpSyntaxTree.ParseText("namespace { class TestClass { } }", new CSharpParseOptions(LanguageVersion.CSharp13))], ReferenceAssemblies.NetStandard.NetStandard20.ResolveAsync(null, default).Result, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var csharpGeneratorDriver = CSharpGeneratorDriver.Create(new FeatureGenerator());
        var generatorDriver = csharpGeneratorDriver.AddAdditionalTexts([MyText.From(
            """
            @UseExtensions true
            @UseRealCondition true
            System.Diagnostics.CodeAnalysis.AllowNullAttribute
            ;Comment
            System
                Single
                    *
                Reflection
                    AssemblyMetadataAttribute
                Runtime.CompilerServices public
                    *
                Diagnostics.CodeAnalysis
                    DisallowNullAttribute public
                IO
                    Stream
                        Read() public
                        Write() public
                ArgumentNullException public
                    ThrowIfNull() public
                ObjectDisposedException
                    ThrowIf()
            """, FeatureGenerator.FileName)]);
        generatorDriver = generatorDriver.RunGenerators(compilation);
        var runResult = generatorDriver.GetRunResult();
        var result = runResult.Results[0];
        System.Console.WriteLine(result.GeneratedSources[0].SourceText);
    }
}
