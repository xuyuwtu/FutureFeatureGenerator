namespace System.Runtime.InteropServices;

[RequireType(nameof(System.Runtime.InteropServices.StringMarshalling))]
#if !NET7_0_OR_GREATER
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
internal sealed class LibraryImportAttribute : Attribute
{
    public LibraryImportAttribute(string libraryName)
    {
        LibraryName = libraryName;
    }
    public string LibraryName { get; }
    public string? EntryPoint { get; set; }
    public StringMarshalling StringMarshalling { get; set; }
    public Type? StringMarshallingCustomType { get; set; }
    public bool SetLastError { get; set; }
}
#endif