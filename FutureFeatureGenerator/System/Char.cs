// 9 IsAscii()
// 18 IsBetween()
// 27 IsAsciiDigit()
namespace System;

internal static partial class FutureChar
{
    // NET6_0_OR_GREATER
    // 5
#if true
    public static bool IsAscii(char c)
    {
        return (uint)c <= '\x007f';
    }
#endif

    // NET7_0_OR_GREATER
    // 5
#if true
    public static bool IsBetween(char c, char minInclusive, char maxInclusive)
    {
        return (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
    }
#endif

    // NET7_0_OR_GREATER
    // 5
    // System.Char.IsBetween()
#if true
    public static bool IsAsciiDigit(char c)
    {
        return IsBetween(c, '0', '9');
    }
#endif
}
