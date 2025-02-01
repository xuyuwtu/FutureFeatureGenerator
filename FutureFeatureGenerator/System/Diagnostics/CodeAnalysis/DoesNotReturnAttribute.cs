namespace System.Diagnostics.CodeAnalysis;
/// <see cref="CSharpFeatureNames.None"/>
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class DoesNotReturnAttribute : Attribute { }
#endif