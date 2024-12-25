// 8.0
// System.Diagnostics.CodeAnalysis.NotNullWhenAttribute.cs
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
internal readonly struct Index : IEquatable<Index>
{
    private readonly int _value;
    #if (NETCOREAPP || NETSTANDARD || NET45_OR_GREATER)
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public Index(int value, bool fromEnd = false)
    {
        if (value < 0)
        {
            ThrowValueArgumentOutOfRange_NeedNonNegNumException();
        }

        if (fromEnd)
            _value = ~value;
        else
            _value = value;
    }
    private Index(int value)
    {
        _value = value;
    }
    public static Index Start => new Index(0);
    public static Index End => new Index(~0);
    #if (NETCOREAPP || NETSTANDARD || NET45_OR_GREATER)
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public static Index FromStart(int value)
    {
        if (value < 0)
        {
            ThrowValueArgumentOutOfRange_NeedNonNegNumException();
        }

        return new Index(value);
    }
    #if (NETCOREAPP || NETSTANDARD || NET45_OR_GREATER)
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public static Index FromEnd(int value)
    {
        if (value < 0)
        {
            ThrowValueArgumentOutOfRange_NeedNonNegNumException();
        }

        return new Index(~value);
    }
    public int Value
    {
        get
        {
            if (_value < 0)
                return ~_value;
            else
                return _value;
        }
    }
    public bool IsFromEnd => _value < 0;
    #if (NETCOREAPP || NETSTANDARD || NET45_OR_GREATER)
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public int GetOffset(int length)
    {
        int offset = _value;
        if (IsFromEnd)
        {
            offset += length + 1;
        }
        return offset;
    }
    public override bool Equals([Diagnostics.CodeAnalysis.NotNullWhen(true)] object? value) => value is Index && _value == ((Index)value)._value;
    public bool Equals(Index other) => _value == other._value;
    public override int GetHashCode() => _value;
    public static implicit operator Index(int value) => FromStart(value);
    public override string ToString()
    {
        if (IsFromEnd)
            return ToStringFromEnd();

        return ((uint)Value).ToString();
    }
    private static void ThrowValueArgumentOutOfRange_NeedNonNegNumException()
    {
        throw new ArgumentOutOfRangeException("value", "value must be non-negative");
    }
    private string ToStringFromEnd()
    {
        return '^' + Value.ToString();
    }
}
#endif