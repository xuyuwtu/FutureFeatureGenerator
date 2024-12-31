// 7 ThrowIfNull()
namespace System;
#pragma warning disable format
internal static class FutureArgumentNullException
{
// NET6_0_OR_GREATER
// 10.0
// System.Diagnostics.CodeAnalysis.NotNullWhenAttribute.cs
// System.Runtime.CompilerServices.CallerArgumentExpressionAttribute.cs
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
