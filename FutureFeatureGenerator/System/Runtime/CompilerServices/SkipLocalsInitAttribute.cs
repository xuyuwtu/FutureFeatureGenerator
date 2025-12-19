namespace System.Runtime.CompilerServices;

[LangVersion(FutureCSharpLanguageVersion.CSharp9)]
#if !NET5_0_OR_GREATER
[AttributeUsage(AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property| AttributeTargets.Event, Inherited = false)]
internal sealed class SkipLocalsInitAttribute : Attribute { }
#endif