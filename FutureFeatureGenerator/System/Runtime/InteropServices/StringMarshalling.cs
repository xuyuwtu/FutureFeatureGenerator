namespace System.Runtime.InteropServices;

#if !NET7_0_OR_GREATER
internal enum StringMarshalling
{
    Custom = 0,
    Utf8,
    Utf16
}
#endif