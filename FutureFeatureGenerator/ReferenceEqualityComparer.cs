using System.Diagnostics.CodeAnalysis;

namespace FutureFeatureGenerator;

internal class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
{
    public static ReferenceEqualityComparer<T> Instance { get; } = new ReferenceEqualityComparer<T>();
    public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
    public int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
}
