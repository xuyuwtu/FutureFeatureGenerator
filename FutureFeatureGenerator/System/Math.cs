#region
#endregion
namespace System;

internal static partial class FutureMath
{
    #region Clamp(byte,byte,byte)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static byte Clamp(byte value, byte min, byte max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(decimal,decimal,decimal)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static decimal Clamp(decimal value, decimal min, decimal max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(double,double,double)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static double Clamp(double value, double min, double max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(short,short,short)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static short Clamp(short value, short min, short max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(int,int,int)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static int Clamp(int value, int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(long,long,long)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static long Clamp(long value, long min, long max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(sbyte,sbyte,sbyte)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static sbyte Clamp(sbyte value, sbyte min, sbyte max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(float,float,float)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static float Clamp(float value, float min, float max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(ushort,ushort,ushort)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static ushort Clamp(ushort value, ushort min, ushort max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(uint,uint,uint)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static uint Clamp(uint value, uint min, uint max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(ulong,ulong,ulong)
    [Alias(nameof(Clamp))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static ulong Clamp(ulong value, ulong min, ulong max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(nint,nint,nint)
    [Alias(nameof(Clamp))]
#if !NET6_0_OR_GREATER
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static nint Clamp(nint value, nint min, nint max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion

    #region Clamp(nuint,nuint,nuint)
    [Alias(nameof(Clamp))]
#if !NET6_0_OR_GREATER
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static nuint Clamp(nuint value, nuint min, nuint max)
    {
        if (min > max)
        {
            throw new ArgumentException(string.Format("'{0}' cannot be greater than {1}.", min, max));
        }
        if (value < min)
        {
            return min;
        }
        else if (value > max)
        {
            return max;
        }
        return value;
    }
#endif
    #endregion
}
