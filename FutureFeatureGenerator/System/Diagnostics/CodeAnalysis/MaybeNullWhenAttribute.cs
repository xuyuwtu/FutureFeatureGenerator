// 8.0
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class MaybeNullWhenAttribute : Attribute
{
    public bool ReturnValue { get; }
    public MaybeNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }
}
#endif