using Microsoft.CodeAnalysis;

namespace FutureFeatureGenerator;

internal sealed class CompilationExternalReferencesEqualityComparer : IEqualityComparer<Compilation>
{
    public static CompilationExternalReferencesEqualityComparer Instance { get; } = new();
    private CompilationExternalReferencesEqualityComparer() { }
    public bool Equals(Compilation? x, Compilation? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }
        if(x is null || y is null)
        {
            return false;
        }
        var xExternalReferences = x.ExternalReferences.OfType<PortableExecutableReference>().ToArray();
        var yExternalReferences = y.ExternalReferences.OfType<PortableExecutableReference>().ToArray();
        if(xExternalReferences.Length != yExternalReferences.Length)
        {
            return false;
        }
        var count = xExternalReferences.Length;
        var comparison = Environment.OSVersion.Platform == PlatformID.Win32NT ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        for (int i = 0; i < count; i++)
        {
            if (!string.Equals(xExternalReferences[i].FilePath, yExternalReferences[i].FilePath, comparison))
            {
                return false;
            }
        }
        return true;
    }
    public int GetHashCode(Compilation obj) => obj.GetHashCode();
}