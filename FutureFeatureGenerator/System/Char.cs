#region
#endregion
namespace System;

internal static partial class FutureChar
{
    // NET6_0_OR_GREATER
    #region IsAscii()
    /// <see cref="CSharpFeatureNames.None"/>
#if true
    internal static bool IsAscii(char c)
    {
        return (uint)c <= '\x007f';
    }
#endif
    #endregion

    // NET7_0_OR_GREATER
    #region IsBetween()
    /// <see cref="CSharpFeatureNames.None"/>
#if true
    internal static bool IsBetween(char c, char minInclusive, char maxInclusive)
    {
        return (uint)(c - minInclusive) <= (uint)(maxInclusive - minInclusive);
    }
#endif
    #endregion

    // NET7_0_OR_GREATER
    #region IsAsciiDigit()
    /// <see cref="CSharpFeatureNames.None"/>
    // System.Char.IsBetween()
#if true
    internal static bool IsAsciiDigit(char c)
    {
        return IsBetween(c, '0', '9');
    }
#endif
    #endregion
}
