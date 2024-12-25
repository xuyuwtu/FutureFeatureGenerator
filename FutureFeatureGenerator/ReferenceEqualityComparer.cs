using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FutureFeatureGenerator;

internal class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
{
    public static ReferenceEqualityComparer<T> Instance { get; } = new ReferenceEqualityComparer<T>();
    public bool Equals(T? x, T? y) => ReferenceEquals(x, y);
#pragma warning disable RS1024 // 正确比较符号
    public int GetHashCode([DisallowNull] T obj) => RuntimeHelpers.GetHashCode(obj);
#pragma warning restore RS1024 // 正确比较符号
}
