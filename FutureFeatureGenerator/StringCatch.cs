using System.Text;

namespace FutureFeatureGenerator;

internal class StringCache
{
    private Dictionary<string, string> cache = new(StringComparer.Ordinal);
    private ReferenceList<string> references = new();
    private Encoding _encoding;
    public Encoding Encoding => _encoding;
    public StringCache(Encoding? encoding = null)
    {
        _encoding = encoding ?? Encoding.UTF8;
    }
    public string GetOrAdd(string s)
    {
        var index = references.IndexOf(s);
        if (index != -1)
        {
            return references[index];
        }
        if (cache.TryGetValue(s, out var result))
        {
            return result;
        }
        cache.Add(s, s);
        references.Add(s);
        return s;
    }
    public int IndexOf(string s) => references.IndexOf(s);
    public bool Exists(string s) => IndexOf(s) != -1;
}
