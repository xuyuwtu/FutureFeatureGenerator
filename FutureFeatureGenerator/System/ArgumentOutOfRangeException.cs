// 12 ThrowIfEqual()
// 25 ThrowIfNotEqual()
// 38 ThrowIfGreaterThan()
// 51 ThrowIfGreaterThanOrEqual()
// 64 ThrowIfLessThan()
// 77 ThrowIfLessThanOrEqual()
namespace System;
#pragma warning disable format
internal static class FutureArgumentOutOfRangeException
{
// NET8_0_OR_GREATER
// 10.0
// System.Runtime.CompilerServices.CallerArgumentExpressionAttribute.cs
#if true
internal static void ThrowIfEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
{
    if (Collections.Generic.EqualityComparer<T>.Default.Equals(value, other))
    {
        throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must not be equal to '{2}'.", paramName, ((object?)value) ?? "null", ((object?)other) ?? "null"));
    }
}
#endif

// NET8_0_OR_GREATER
// 10.0
// System.Runtime.CompilerServices.CallerArgumentExpressionAttribute.cs
#if true
internal static void ThrowIfNotEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
{
    if (!Collections.Generic.EqualityComparer<T>.Default.Equals(value, other))
    {
        throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be equal to '{2}'.", paramName, ((object?)value) ?? "null", ((object?)other) ?? "null"));
    }
}
#endif

// NET8_0_OR_GREATER
// 10.0
// System.Runtime.CompilerServices.CallerArgumentExpressionAttribute.cs
#if true
internal static void ThrowIfGreaterThan<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
{
    if (value.CompareTo(other) > 0)
    {
        throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be less than or equal to '{2}'.", paramName, value, other));
    }
}
#endif

// NET8_0_OR_GREATER
// 10.0
// System.Runtime.CompilerServices.CallerArgumentExpressionAttribute.cs
#if true
internal static void ThrowIfGreaterThanOrEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
{
    if (value.CompareTo(other) >= 0)
    {
        throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be less than '{2}'.", paramName, value, other));
    }
}
#endif

// NET8_0_OR_GREATER
// 10.0
// System.Runtime.CompilerServices.CallerArgumentExpressionAttribute.cs
#if true
internal static void ThrowIfLessThan<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
{
    if (value.CompareTo(other) < 0)
    {
        throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be greater than or equal to '{2}'.", paramName, value, other));
    }
}
#endif

// NET8_0_OR_GREATER
// 10.0
// System.Runtime.CompilerServices.CallerArgumentExpressionAttribute.cs
#if true
internal static void ThrowIfLessThanOrEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
{
    if (value.CompareTo(other) <= 0)
    {
        throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be greater than '{2}'.", paramName, value, other));
    }
}
#endif
}
