#region
#endregion
namespace System;
internal static partial class FutureArgumentOutOfRangeException
{
    #region ThrowIfEqual()
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
    [RealCondition("!NET8_0_OR_GREATER")]
#if true
    internal static void ThrowIfEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
    {
        if (Collections.Generic.EqualityComparer<T>.Default.Equals(value, other))
        {
            throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must not be equal to '{2}'.", paramName, ((object?)value) ?? "null", ((object?)other) ?? "null"));
        }
    }
#endif
    #endregion

    #region ThrowIfNotEqual()
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
    [RealCondition("!NET8_0_OR_GREATER")]
#if true
    internal static void ThrowIfNotEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IEquatable<T>?
    {
        if (!Collections.Generic.EqualityComparer<T>.Default.Equals(value, other))
        {
            throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be equal to '{2}'.", paramName, ((object?)value) ?? "null", ((object?)other) ?? "null"));
        }
    }
#endif
    #endregion

    #region ThrowIfGreaterThan()
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
    [RealCondition("!NET8_0_OR_GREATER")]
#if true
    internal static void ThrowIfGreaterThan<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be less than or equal to '{2}'.", paramName, value, other));
        }
    }
#endif
    #endregion

    #region ThrowIfGreaterThanOrEqual()
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
    [RealCondition("!NET8_0_OR_GREATER")]
#if true
    internal static void ThrowIfGreaterThanOrEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(other) >= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be less than '{2}'.", paramName, value, other));
        }
    }
#endif
    #endregion

    #region ThrowIfLessThan()
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
    [RealCondition("!NET8_0_OR_GREATER")]
#if true
    internal static void ThrowIfLessThan<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be greater than or equal to '{2}'.", paramName, value, other));
        }
    }
#endif
    #endregion

    #region ThrowIfLessThanOrEqual()
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
    [RealCondition("!NET8_0_OR_GREATER")]
#if true
    internal static void ThrowIfLessThanOrEqual<T>(T value, T other, [Runtime.CompilerServices.CallerArgumentExpression(nameof(value))] string? paramName = null) where T : IComparable<T>
    {
        if (value.CompareTo(other) <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, value, string.Format("{0} ('{1}') must be greater than '{2}'.", paramName, value, other));
        }
    }
#endif
    #endregion
}
