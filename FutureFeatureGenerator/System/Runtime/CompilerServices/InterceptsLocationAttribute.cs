namespace System.Runtime.CompilerServices;

#if true
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class InterceptsLocationAttribute : Attribute
{
    public InterceptsLocationAttribute(int version, string data) { }
}
#endif