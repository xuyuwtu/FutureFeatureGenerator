namespace System.Runtime.CompilerServices;

[LangVersion(FutureCSharpLanguageVersion.CSharp15)]
#if !NET11_0_OR_GREATER
internal interface IUnion
{
    object Value { get; }
}
#endif