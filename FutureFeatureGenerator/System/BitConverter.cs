#region
#endregion

namespace System;

internal static partial class FutureBitConverter
{
    #region ToBoolean()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static bool ToBoolean(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(byte))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<byte>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value)) != 0;
    }
#endif
    #endregion

    #region ToChar()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static char ToChar(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(char))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<char>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToDouble()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToDouble(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(double))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<double>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToInt16()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToInt16(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(short))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<short>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToInt32()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToInt32(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(int))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToInt64()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToInt64(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(long))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<long>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToSingle()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToSingle(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(float))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<float>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToUInt16()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToUInt16(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(ushort))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<ushort>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToUInt32()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToUInt32(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(uint))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region ToUInt64()
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double ToUInt64(ReadOnlySpan<byte> value)
    {
        if (value.Length < sizeof(ulong))
        {
            throw new ArgumentOutOfRangeException("value");
        }
        return Runtime.CompilerServices.Unsafe.ReadUnaligned<ulong>(ref Runtime.InteropServices.MemoryMarshal.GetReference(value));
    }
#endif
    #endregion

    #region TryWriteBytes(bool)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, bool value)
    {
        if (destination.Length < sizeof(byte))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value ? (byte)1 : (byte)0);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(char)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, char value)
    {
        if (destination.Length < sizeof(char))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(double)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, double value)
    {
        if (destination.Length < sizeof(double))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(short)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, short value)
    {
        if (destination.Length < sizeof(short))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(int)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, int value)
    {
        if (destination.Length < sizeof(int))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(long)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, long value)
    {
        if (destination.Length < sizeof(long))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(float)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, float value)
    {
        if (destination.Length < sizeof(float))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(ushort)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, ushort value)
    {
        if (destination.Length < sizeof(ushort))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(uint)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, uint value)
    {
        if (destination.Length < sizeof(uint))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion

    #region TryWriteBytes(ulong)
    [Alias(nameof(TryWriteBytes))]
    [RealCondition("!(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)")]
#if true
    internal static bool TryWriteBytes(Span<byte> destination, ulong value)
    {
        if (destination.Length < sizeof(ulong))
        {
            return false;
        }
        Runtime.CompilerServices.Unsafe.WriteUnaligned(ref Runtime.InteropServices.MemoryMarshal.GetReference(destination), value);
        return true;
    }
#endif
    #endregion
}
