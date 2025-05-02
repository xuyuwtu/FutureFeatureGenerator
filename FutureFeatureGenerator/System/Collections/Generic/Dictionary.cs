#region
#endregion
namespace System.Collections.Generic;

internal static partial class FutureDictionary
{
    #region TryAdd()
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
    {
        if (self.ContainsKey(key))
        {
            return false;
        }
        self.Add(key, value);
        return true;
    }
#endif
    #endregion
}
