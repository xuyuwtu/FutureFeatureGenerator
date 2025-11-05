#region
#endregion
namespace System;
internal static partial class FutureArgumentNullException
{
    #region ThrowIfNull()
    [RequireType(nameof(System.Diagnostics.CodeAnalysis.NotNullAttribute))]
    [RequireType(nameof(System.Runtime.CompilerServices.CallerArgumentExpressionAttribute))]
#if !NET6_0_OR_GREATER
    internal static void ThrowIfNull([Diagnostics.CodeAnalysis.NotNull] object? argument, [Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
#endif
    #endregion
}
