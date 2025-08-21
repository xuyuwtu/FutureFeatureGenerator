using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace FutureFeatureGenerator.Console.Test;

internal class Program
{
    static void Main(string[] args)
    {
        var compilation = CSharpCompilation.Create(
            "Test", 
            [CSharpSyntaxTree.ParseText("namespace { class TestClass { } }", new CSharpParseOptions(
                LanguageVersion.CSharp13, 
                preprocessorSymbols: ["NETCOREAPP3_0_OR_GREATER", "NET5_0_OR_GREATER", "NET6_0_OR_GREATER"]))], 
            ReferenceAssemblies.Net.Net60.ResolveAsync(null, default).Result, 
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var csharpGeneratorDriver = CSharpGeneratorDriver.Create(new FeatureGenerator());
        var generatorDriver = csharpGeneratorDriver.AddAdditionalTexts([MyText.From(
            """
            @UseExtensions true
            @UseRealCondition true
            System public
                Char
                    *
                Range
                Index
                Runtime.CompilerServices
                    *
            ThrowIf()
            ThrowIfEqual()
            """, FeatureGenerator.FileName)]);
        generatorDriver = generatorDriver.RunGenerators(compilation);
        var runResult = generatorDriver.GetRunResult();
        var result = runResult.Results[0];
        if (result.Diagnostics.Length != 0)
        {
            Debugger.Break();
            foreach(var dia in runResult.Diagnostics)
            {
                System.Console.WriteLine(dia);
            }
        }
        else
        {
            System.Console.WriteLine(result.GeneratedSources[0].SourceText);
        }
    }
}
