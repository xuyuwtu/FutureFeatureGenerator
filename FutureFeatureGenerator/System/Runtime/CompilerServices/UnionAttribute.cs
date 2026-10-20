namespace System.Runtime.CompilerServices;

[LangVersion(FutureCSharpLanguageVersion.CSharp15)]
#if !NET11_0_OR_GREATER
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class UnionAttribute : Attribute { }
#endif