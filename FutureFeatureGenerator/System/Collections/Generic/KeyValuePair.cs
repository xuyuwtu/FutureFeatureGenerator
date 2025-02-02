#region
#endregion
namespace System.Collections.Generic;

internal static partial class FutureKeyValuePair
{
    #region Deconstruct()
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static void Deconstruct<TKey, TValue>(this in KeyValuePair<TKey, TValue> keyValuePair, out TKey key, out TValue value)
    {
        key = keyValuePair.Key;
        value = keyValuePair.Value;
    }
#endif
    #endregion
}
