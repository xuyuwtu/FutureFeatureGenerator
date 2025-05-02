#region
#endregion
namespace System;

internal static partial class FutureSingle
{
    #region IsFinite()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsFinite(float f)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        int bits = BitConverter.SingleToInt32Bits(f);
        return (bits & 0x7FFFFFFF) < 0x7F800000;
#else
        unsafe
        {
            int bits = *(int*)&f;
            return (bits & 0x7FFFFFFF) < 0x7F800000;
        }
#endif
    }
#endif
    #endregion

    #region IsNegative()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsNegative(float f)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        return BitConverter.SingleToInt32Bits(f) < 0;
#else
        unsafe
        {
            return *(int*)&f < 0;
        }
#endif
    }
#endif
    #endregion

    #region IsNormal()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsNormal(float f)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        int bits = BitConverter.SingleToInt32Bits(f);
            bits &= 0x7FFFFFFF;
            return (bits < 0x7F800000) && (bits != 0) && ((bits & 0x7F800000) != 0);
#else
        unsafe
        {
            int bits = *(int*)&f;
            bits &= 0x7FFFFFFF;
            return (bits < 0x7F800000) && (bits != 0) && ((bits & 0x7F800000) != 0);
        }
#endif
    }
#endif
    #endregion

    #region IsSubnormal()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool IsSubnormal(float f)
    {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        int bits = BitConverter.SingleToInt32Bits(f);
        bits &= 0x7FFFFFFF;
        return (bits < 0x7F800000) && (bits != 0) && ((bits & 0x7F800000) == 0);
#else
        unsafe
        {
            int bits = *(int*)&f;
            bits &= 0x7FFFFFFF;
            return (bits < 0x7F800000) && (bits != 0) && ((bits & 0x7F800000) == 0);
        }
#endif
    }
#endif
    #endregion
}
