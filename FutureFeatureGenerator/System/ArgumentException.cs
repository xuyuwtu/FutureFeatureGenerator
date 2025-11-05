#region
#endregion
namespace System;
internal static partial class FutureArgumentException
{
    #region ThrowIfNullOrEmpty()
    [RequireType(nameof(System.Diagnostics.CodeAnalysis.NotNullAttribute))]
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
#if !NET7_0_OR_GREATER
    internal static void ThrowIfNullOrEmpty([Diagnostics.CodeAnalysis.NotNull] string? argument, [Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentException("The value cannot be an empty string.", paramName);
        }
    }
#endif
    #endregion

    #region ThrowIfNullOrWhiteSpace()
    [RequireType(nameof(System.Diagnostics.CodeAnalysis.NotNullAttribute))]
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
#if !NET8_0_OR_GREATER
    internal static void ThrowIfNullOrWhiteSpace([Diagnostics.CodeAnalysis.NotNull] string? argument, [Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException("The value cannot be an empty string or composed entirely of whitespace.", paramName);
        }
    }
#endif
    #endregion
}