#region
#endregion
namespace System;

internal static partial class FutureObjectDisposedException
{
    #region ThrowIf(Boolean,Object)
    [RequireType(nameof(System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute))]
    [Alias(nameof(ThrowIf))]
    [RealCondition("!NET7_0_OR_GREATER")]
#if true
#if NET6_0_OR_GREATER
    [Diagnostics.StackTraceHidden]
#endif
    internal static void ThrowIf([Diagnostics.CodeAnalysis.DoesNotReturnIf(true)] bool condition, object instance)
    {
        if (condition)
        {
            throw new ObjectDisposedException(instance?.GetType().FullName);
        }
    }
#endif
    #endregion

    #region ThrowIf(Boolean,Type)
    [RequireType(nameof(System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute))]
    [Alias(nameof(ThrowIf))]
    [RealCondition("!NET7_0_OR_GREATER")]
#if true
#if NET6_0_OR_GREATER
    [Diagnostics.StackTraceHidden]
#endif
    internal static void ThrowIf([Diagnostics.CodeAnalysis.DoesNotReturnIf(true)] bool condition, Type type)
    {
        if (condition)
        {
            throw new ObjectDisposedException(type?.FullName);
        }
    }
#endif
    #endregion
}
