using System.Collections.Concurrent;

namespace FutureFeatureGenerator;

internal class StringCache
{
    private ConcurrentDictionary<string, string> cache = new(StringComparer.Ordinal);
    public StringCache()
    {
    }
    public string GetOrAdd(string s)
    {
        return cache.GetOrAdd(s, s);
    }
    public bool Exists(string s)
    {
        return cache.ContainsKey(s);
    }
}
