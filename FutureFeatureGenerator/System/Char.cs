#region
#endregion
namespace System;

internal static partial class FutureChar
{
    #region IsAscii()
    [RealCondition("!NET6_0_OR_GREATER")]
#if true
    internal static bool IsAscii(char c)
    {
        return (uint)c <= '\x007f';
    }
#endif
    #endregion

    #region IsBetween()
    [RealCondition("!NET7_0_OR_GREATER")]
#if true
    internal static bool IsBetween(char c, char minInclusive, char maxInclusive)
    {
        return (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
    }
#endif
    #endregion

    #region IsAsciiDigit()
    // System.Char.IsBetween()
    [RealCondition("!NET7_0_OR_GREATER")]
#if true
    internal static bool IsAsciiDigit(char c)
    {
        return IsBetween(c, '0', '9');
    }
#endif
    #endregion
}
