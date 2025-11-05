#region
#endregion
namespace System;

internal static partial class FutureChar
{
    #region IsAscii()
#if !NET6_0_OR_GREATER
    internal static bool IsAscii(char c)
    {
        return (uint)c <= '\x007f';
    }
#endif
    #endregion

    #region IsBetween()
#if !NET7_0_OR_GREATER
    internal static bool IsBetween(char c, char minInclusive, char maxInclusive)
    {
        return (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
    }
#endif
    #endregion

    #region IsAsciiDigit()
    // System.Char.IsBetween()
#if !NET7_0_OR_GREATER
    internal static bool IsAsciiDigit(char c)
    {
        return IsBetween(c, '0', '9');
    }
#endif
    #endregion
}
