using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using System.Runtime.CompilerServices;

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
}