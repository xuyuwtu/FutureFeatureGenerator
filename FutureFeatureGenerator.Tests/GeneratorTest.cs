using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;

namespace FutureFeatureGenerator.Tests;

public class GeneratorTest
{
    ImmutableArray<MetadataReference> NetStandard20WithIndexRangeReferences;
    ImmutableArray<MetadataReference> NetStandard20References;
    public GeneratorTest()
    {
        NetStandard20WithIndexRangeReferences = ReferenceAssemblies.NetStandard.NetStandard20.AddPackages([new PackageIdentity("IndexRange", "1.0.3")]).ResolveAsync(null, default).Result;
        NetStandard20References = NetStandard20WithIndexRangeReferences.RemoveAll(static x => Path.GetFileName(x.Display) == "IndexRange.dll");
    }
    [Fact]
    public void TypeCheck_Range_Resolve3Type()
    {
        Assert.True(Utils.PerfectMatch(Utils.GetGeneratorTypeFullNames($"System.{nameof(Range)} public", NetStandard20References), nameof(Range), nameof(Index), nameof(NotNullWhenAttribute)));
    }
    [Fact]
    public void TypeCheck_RangeHasIndexRange_ResolveEmpty()
    {
        Assert.True(Utils.PerfectMatch(Utils.GetGeneratorTypeFullNames($"System.{nameof(Range)}", NetStandard20WithIndexRangeReferences)));
    }
    [Fact]
    public void TypeCheck_RequireLanguageVersion_ResolveEmpty()
    {
        Assert.True(Utils.PerfectMatch(Utils.GetGeneratorTypeFullNames("System.Diagnostics.CodeAnalysis.*", NetStandard20References, LanguageVersion.CSharp7_3)));
    }
    [Fact]
    public void TypeCheck_RequireLanguageVersionSuccess_ResolveSuccess()
    {
        Assert.True(Utils.AllMatch(Utils.GetGeneratorTypeFullNames("System.Diagnostics.CodeAnalysis.*", NetStandard20References, LanguageVersion.CSharp8),
            nameof(AllowNullAttribute),
            nameof(DisallowNullAttribute),
            nameof(DoesNotReturnAttribute),
            nameof(DoesNotReturnIfAttribute),
            nameof(MaybeNullAttribute),
            nameof(MaybeNullWhenAttribute),
            nameof(NotNullAttribute),
            nameof(NotNullIfNotNullAttribute),
            nameof(NotNullWhenAttribute)));
    }
}