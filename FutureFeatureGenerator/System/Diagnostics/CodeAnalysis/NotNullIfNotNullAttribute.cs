namespace System.Diagnostics.CodeAnalysis;
// 8.0
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute : Attribute
{
    public string ParameterName { get; }
    public NotNullIfNotNullAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }
}
#endif