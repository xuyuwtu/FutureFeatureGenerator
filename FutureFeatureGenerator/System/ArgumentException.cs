#region
#endregion
namespace System;
internal static partial class FutureArgumentException
{
    // NET7_0_OR_GREATER
    #region ThrowIfNullOrEmpty()
    // System.Diagnostics.CodeAnalysis.NotNullWhenAttribute
    // System.Runtime.CompilerServices.CallerArgumentExpressionAttribute
#if true
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

    // NET8_0_OR_GREATER
    #region ThrowIfNullOrWhiteSpace()
    // System.Diagnostics.CodeAnalysis.NotNullWhenAttribute
    // System.Runtime.CompilerServices.CallerArgumentExpressionAttribute
#if true
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