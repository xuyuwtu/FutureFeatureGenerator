#region
#endregion
namespace System.Collections.Generic;

internal static partial class FutureCollectionExtensions
{
    #region AsReadOnly(IList<T>)
    [Alias(nameof(AsReadOnly))]
#if !NET7_0_OR_GREATER
    internal static ObjectModel.ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> list)
    {
        return new ObjectModel.ReadOnlyCollection<T>(list);
    }
#endif
    #endregion

    #region AsReadOnly(IDictionary<TKey,TValue>)
#if !NET7_0_OR_GREATER
    internal static ObjectModel.ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TKey : notnull
    {
        return new ObjectModel.ReadOnlyDictionary<TKey, TValue>(dictionary);
    }
#endif
    #endregion

    #region GetValueOrDefault(IReadOnlyDictionary<TKey,TValue>,TKey)
    [Alias(nameof(GetValueOrDefault))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.GetValueOrDefault(key, default!);
    }
#endif
    #endregion

    #region GetValueOrDefault(IReadOnlyDictionary<TKey,TValue>,TKey,TValue)
    [Alias(nameof(GetValueOrDefault))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }
        return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
    }
#endif
    #endregion

    #region Remove(IDictionary<TKey,TValue>,TKey,out-TValue)
    [Alias(nameof(Remove))]
    [RequireType(nameof(System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, [Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }
        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            return true;
        }
        value = default;
        return false;
    }
#endif
    #endregion

    #region TryAdd(IDictionary<TKey,TValue>,TKey,TValue)
    [Alias(nameof(TryAdd))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }
        return false;
    }
#endif
    #endregion
}
