namespace System.Diagnostics.CodeAnalysis;

[LangVersion(FutureCSharpLanguageVersion.CSharp11)]
#if !NET7_0_OR_GREATER
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false,Inherited = false)]
internal sealed class UnscopedRefAttribute : Attribute { }
#endif