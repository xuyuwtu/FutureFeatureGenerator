namespace System.Diagnostics.CodeAnalysis;
/// <see cref="CSharpFeatureNames.AutomaticProperties"/>
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : Attribute
{
    public bool ReturnValue { get; }
    public NotNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }
}
#endif