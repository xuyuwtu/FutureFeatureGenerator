#region
#endregion
namespace System;

internal static partial class FutureString
{
    #region EndsWith(char)
    [Alias(nameof(EndsWith))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static bool EndsWith(this string self, char value)
    {
        int num = self.Length - 1;
        if ((uint)num < (uint)self.Length)
        {
            return self[num] == value;
        }
        return false;
    }
#endif
    #endregion

    #region GetHashCode(StringComparison)
    // System.StringComparer.FromComparison(StringComparison)
    [Alias(nameof(GetHashCode))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static int GetHashCode(this string self, StringComparison comparisonType)
    {
        return FutureStringComparer.FromComparison(comparisonType).GetHashCode(self);
    }
#endif
    #endregion


    #region IndexOf(char,StringComparison)
    [Alias(nameof(IndexOf))]
#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static int IndexOf(this string self, char value, StringComparison comparisonType)
    {
        switch (comparisonType)
        {
            case StringComparison.CurrentCulture:
            case StringComparison.CurrentCultureIgnoreCase:
                return Globalization.CultureInfo.CurrentCulture.CompareInfo.IndexOf(self, value, (Globalization.CompareOptions)(comparisonType & StringComparison.CurrentCultureIgnoreCase));
            case StringComparison.InvariantCulture:
            case StringComparison.InvariantCultureIgnoreCase:
                return Globalization.CultureInfo.InvariantCulture.CompareInfo.IndexOf(self, value, (Globalization.CompareOptions)(comparisonType & StringComparison.CurrentCultureIgnoreCase));
            case StringComparison.Ordinal:
                return self.IndexOf(value);
            case StringComparison.OrdinalIgnoreCase:
                return Globalization.CultureInfo.InvariantCulture.CompareInfo.IndexOf(self, value, Globalization.CompareOptions.OrdinalIgnoreCase);
            default:
                throw new ArgumentException("The string comparison type passed in is currently not supported.", nameof(comparisonType));
        }
    }
#endif
    #endregion

    #region StartsWith(char)
    [Alias(nameof(StartsWith))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static bool StartsWith(this string self, char value)
    {
        if (self.Length != 0)
        {
            return self[0] == value;
        }
        return false;
    }
#endif
    #endregion

}
