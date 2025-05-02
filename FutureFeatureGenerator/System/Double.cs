#region
#endregion
namespace System;

internal static partial class FutureDouble
{
    #region IsFinite()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
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

    #region IsNegative()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
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

    #region IsNormal()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
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

    #region IsSubnormal()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
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
