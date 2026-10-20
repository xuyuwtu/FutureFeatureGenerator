using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace FutureFeatureGenerator.Tests;

public class GeneratorTest
{
    ImmutableArray<MetadataReference> NetStandard20References;
    public GeneratorTest()
    {
        NetStandard20References = ReferenceAssemblies.NetStandard.NetStandard20.ResolveAsync(null, default).Result;
    }
    [Fact]
    public void TypeCheck_Range_Resolve3Type()
    {
        Assert.True(Utils.PerfectMatch(Utils.GetGeneratorTypeFullNames($"System.{nameof(Range)} public", NetStandard20References), nameof(Range), nameof(Index), nameof(NotNullWhenAttribute)));
    }
    [Fact]
    public void OptionCheck_DisableAddDependencies()
    {
        Assert.True(Utils.PerfectMatch(Utils.GetGeneratorTypeFullNames($"""
            @DisableAddDependencies true
            System.{nameof(Range)}
            """, NetStandard20References), nameof(Range)));
    }
    [Fact]
    public void OptionCheck_AutoAddLangType()
    {
        var generatedNames = Utils.GetGeneratorTypeFullNames($"""
            @AutoAddLangType true
            """, NetStandard20References, LanguageVersion.CSharp11);
        string[] shouldGeneratedNames = [
            nameof(CallerFilePathAttribute),
            nameof(CallerLineNumberAttribute),
            nameof(CallerMemberNameAttribute),
            nameof(Index),
            nameof(Range),
            nameof(AllowNullAttribute),
            nameof(DisallowNullAttribute),
            nameof(DoesNotReturnAttribute),
            nameof(DoesNotReturnIfAttribute),
            nameof(MaybeNullAttribute),
            nameof(MaybeNullWhenAttribute),
            nameof(MemberNotNullAttribute),
            nameof(MemberNotNullWhenAttribute),
            nameof(NotNullAttribute),
            nameof(NotNullIfNotNullAttribute),
            nameof(NotNullWhenAttribute),
            nameof(IsExternalInit),
            nameof(ModuleInitializerAttribute),
            nameof(SkipLocalsInitAttribute),
            nameof(CallerArgumentExpressionAttribute),
            nameof(RequiredMemberAttribute),
            nameof(SetsRequiredMembersAttribute),
            nameof(CompilerFeatureRequiredAttribute),
            nameof(UnscopedRefAttribute),
            ];
        Assert.True(Utils.AllMatch(generatedNames, shouldGeneratedNames));
    }
    [Fact]
    public void OptionCheck_NotWriteGeneratedCodeAttribute()
    {
        var driver = Utils.GetGeneratorDriver(nameof(Index), NetStandard20References);
        var result = driver.GetRunResult();
        Assert.Single(result.Results);
        Assert.Single(result.Results[0].GeneratedSources);
        Assert.Matches(nameof(GeneratedCodeAttribute), result.Results[0].GeneratedSources[0].SourceText.ToString());

        driver = Utils.GetGeneratorDriver($"""
            @NotWriteGeneratedCodeAttribute true
            System.{nameof(Index)}
            """, NetStandard20References);
        result = driver.GetRunResult();
        Assert.Single(result.Results);
        Assert.Single(result.Results[0].GeneratedSources);
        Assert.DoesNotMatch(nameof(GeneratedCodeAttribute), result.Results[0].GeneratedSources[0].SourceText.ToString());
    }
    [Fact]
    public void OptionCheck_DefaultModifier()
    {
        var driver = Utils.GetGeneratorDriver($"""
            @DefaultModifier public
            {nameof(Index)}
            """, NetStandard20References);
        var result = driver.GetRunResult();
        Assert.Single(result.Results);
        Assert.Single(result.Results[0].GeneratedSources);
        var text = result.Results[0].GeneratedSources[0].SourceText.ToString();
        Assert.DoesNotMatch("internal readonly", text);
    }
    [Fact]
    public void OptionCheck_UseExtensions_Const()
    {
        var driver = Utils.GetGeneratorDriver($"""
            @UseExtensions true
            System
                Math
                    Tau
                    Clamp(int,int,int)
            """, NetStandard20References);
        var result = driver.GetRunResult();
        Assert.Single(result.Results);
        Assert.Single(result.Results[0].GeneratedSources);
        var text = result.Results[0].GeneratedSources[0].SourceText.ToString();
        var extensionIndex = text.IndexOf("extension(Math)");
        var constIndex = text.IndexOf("const double Tau");
        Assert.True(extensionIndex >= 0);
        Assert.True(constIndex >= 0);
        //const成员不能放进extension块，必须写在extension块外的类主体中
        Assert.True(constIndex < extensionIndex);
    }
    [Fact]
    public void OptionCheck_UseExtensionsLangVersion()
    {
        var fileContent = $"""
            System
                Math
                    Clamp(int,int,int)
            """;
        //C#语言版本为14及以上时UseExtensions默认为true
        var driver = Utils.GetGeneratorDriver(fileContent, NetStandard20References, (LanguageVersion)1400);
        var result = driver.GetRunResult();
        Assert.Single(result.Results);
        Assert.Single(result.Results[0].GeneratedSources);
        var text = result.Results[0].GeneratedSources[0].SourceText.ToString();
        Assert.Contains("extension(Math)", text);
        //默认语言版本低于C#14时UseExtensions为false
        var driverDefault = Utils.GetGeneratorDriver(fileContent, NetStandard20References);
        var resultDefault = driverDefault.GetRunResult();
        Assert.Single(resultDefault.Results);
        Assert.Single(resultDefault.Results[0].GeneratedSources);
        var textDefault = resultDefault.Results[0].GeneratedSources[0].SourceText.ToString();
        Assert.DoesNotContain("extension(", textDefault);
    }
    [Fact]
    public void TypeCheck_NewMember_Alias()
    {
        var generatedNames = Utils.GetGeneratorTypeFullNames($"""
            System
                Type
                    GetMethod()
                    GetConstructor()
                    IsAssignableTo()
            """, NetStandard20References, LanguageVersion.CSharp7_3);
        string[] shouldGeneratedNames = [
                "GetConstructor(BindingFlags,Type[])",
                "GetMethod(BindingFlags,Type[])",
                "IsAssignableTo(Type)",
                nameof(NotNullWhenAttribute),
        ];
        Assert.True(Utils.AllMatch(generatedNames, shouldGeneratedNames));
    }
    [Fact]
    public void TypeCheck_NewMember()
    {
        var generatedNames = Utils.GetGeneratorTypeFullNames($"""
            System
                Type
                    GetConstructor(BindingFlags,Type[])
                    GetMethod(BindingFlags,Type[])
                    IsAssignableTo(Type)
            """, NetStandard20References, LanguageVersion.CSharp7_3);
        string[] shouldGeneratedNames = [
                "GetConstructor(BindingFlags,Type[])",
                "GetMethod(BindingFlags,Type[])",
                "IsAssignableTo(Type)",
                nameof(NotNullWhenAttribute),
        ];
        Assert.True(Utils.AllMatch(generatedNames, shouldGeneratedNames));
    }
}