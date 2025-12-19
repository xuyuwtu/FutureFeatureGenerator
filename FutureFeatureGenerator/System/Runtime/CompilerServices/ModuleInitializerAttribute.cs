namespace System.Runtime.CompilerServices;

[LangVersion(FutureCSharpLanguageVersion.CSharp9)]
#if !NET5_0_OR_GREATER
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
internal sealed class ModuleInitializerAttribute : Attribute { }
#endif