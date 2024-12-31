namespace System.Reflection;
// 5
#if (!NETCOREAPP && !NETSTANDARD && !NET45_OR_GREATER)
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
internal sealed class AssemblyMetadataAttribute : Attribute
{
    public string Key { get; }
    public string? Value { get; }
    public AssemblyMetadataAttribute(string key, string? value)
    {
        Key = key;
        Value = value;
    }
}
#endif