#region
#endregion
namespace System;

internal static partial class FutureDouble
{
    // !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    #region IsFinite()
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsFinite(double d)
    {
        long bits = BitConverter.DoubleToInt64Bits(d);
        return (bits & 0x7FFFFFFFFFFFFFFF) < 0x7FF0000000000000;
    }
#endif
    #endregion

    // !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    #region IsNegative()
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsNegative(double d)
    {
        return BitConverter.DoubleToInt64Bits(d) < 0;
    }
#endif
    #endregion

    // !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    #region IsNormal()
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsNormal(double d)
    {
        long bits = BitConverter.DoubleToInt64Bits(d);
        bits &= 0x7FFFFFFFFFFFFFFF;
        return (bits < 0x7FF0000000000000) && (bits != 0) && ((bits & 0x7FF0000000000000) != 0);
    }
#endif
    #endregion

    // !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    #region IsSubnormal()
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsSubnormal(double d)
    {
        long bits = BitConverter.DoubleToInt64Bits(d);
        bits &= 0x7FFFFFFFFFFFFFFF;
        return (bits < 0x7FF0000000000000) && (bits != 0) && ((bits & 0x7FF0000000000000) == 0);
    }
#endif
    #endregion
}
