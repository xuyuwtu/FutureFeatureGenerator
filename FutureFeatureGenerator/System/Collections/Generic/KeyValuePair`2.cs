#region
#endregion
namespace System.Collections.Generic;

internal static partial class FutureKeyValuePair_2
{
    #region Deconstruct()
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static void Deconstruct<TKey, TValue>(this in KeyValuePair<TKey, TValue> self, out TKey key, out TValue value)
    {
        key = self.Key;
        value = self.Value;
    }
#endif
    #endregion
}
