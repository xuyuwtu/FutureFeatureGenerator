// 6 ThrowIfNull()
namespace System;
internal static partial class FutureArgumentNullException
{
    // NET6_0_OR_GREATER
    // 10.0
    // System.Diagnostics.CodeAnalysis.NotNullWhenAttribute
    // System.Runtime.CompilerServices.CallerArgumentExpressionAttribute
#if true
    internal static void ThrowIfNull([Diagnostics.CodeAnalysis.NotNull] object? argument, [Runtime.CompilerServices.CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
#endif
}
