namespace System.Diagnostics.CodeAnalysis;
/// <see cref="CSharpFeatureNames.None"/>
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
internal sealed class DisallowNullAttribute : Attribute { }
#endif