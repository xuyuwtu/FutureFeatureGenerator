using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace FutureFeatureGenerator.GeneratorBuild.Test;

internal class Program
{
    static void Main(string[] args)
    {
        var parseOptions = new CSharpParseOptions(
                    LanguageVersion.Preview,
                    preprocessorSymbols: ["NETSTANDARD", "NETSTANDARD2_0_OR_GREATER"]);
        var baseCompilation = CSharpCompilation.Create(
            "Test",
            [CSharpSyntaxTree.ParseText("namespace Test { class TestClass { } }", parseOptions)],
            ReferenceAssemblies.NetStandard.NetStandard20.AddPackages([
                new PackageIdentity("System.Memory", "4.5.5")
            ]).ResolveAsync(null, default).Result,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));
        
        var baseGeneratorDriver = CSharpGeneratorDriver.Create(new FeatureGenerator());

        Task<GeneratorDriver>[] tasks = [
            new Task<GeneratorDriver>(() => baseGeneratorDriver.AddAdditionalTexts([MyText.From(
                """
                @UseRealCondition true
                *
                """, FeatureGenerator.FileName)]).RunGenerators(baseCompilation)),
            new Task<GeneratorDriver>(() => baseGeneratorDriver.AddAdditionalTexts([MyText.From(
                """
                @UseRealCondition true
                @UseExtensions true
                *
                """, FeatureGenerator.FileName)]).RunGenerators(baseCompilation)),
        ];
        foreach (var t in tasks)
        {
            t.Start();
        }
        Task.WhenAll(tasks);
        var normalGeneratorDriver = tasks[0].Result;
        var extensionGeneratorDriver = tasks[1].Result;

        // generated SyntaxTree LanguageVersion is CSharp13 not Preview will throw Exception
        //RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var runResult = normalGeneratorDriver.GetRunResult();
        Debug.Assert(runResult.GeneratedTrees.Length != 0);
        var emitCompilation = baseCompilation.AddSyntaxTrees(runResult.GeneratedTrees.Select(x => CSharpSyntaxTree.ParseText(x.GetText(), parseOptions)));
        using var peStream = new MemoryStream();
        var emitResult = emitCompilation.Emit(peStream);
        Debug.Assert(emitResult.Success);
        foreach (var diagnostic in emitResult.Diagnostics)
        {
            Console.WriteLine(diagnostic);
        }

        runResult = extensionGeneratorDriver.GetRunResult();
        Debug.Assert(runResult.GeneratedTrees.Length != 0);
        emitCompilation = baseCompilation.AddSyntaxTrees(runResult.GeneratedTrees.Select(x => CSharpSyntaxTree.ParseText(x.GetText(), parseOptions)));
        peStream.Position = 0;
        peStream.SetLength(0);
        emitResult = emitCompilation.Emit(peStream);
        Debug.Assert(emitResult.Success);
        foreach (var diagnostic in emitResult.Diagnostics)
        {
            Console.WriteLine(diagnostic);
        }
    }
}
