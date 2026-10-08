namespace System.Collections.Generic;
#if !NET5_0_OR_GREATER
internal sealed class ReferenceEqualityComparer : IEqualityComparer<object?>, IEqualityComparer
{
    private ReferenceEqualityComparer() { }
    public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();
    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
    public int GetHashCode(object? obj)
    {
        return Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj!);
    }
}
#endif