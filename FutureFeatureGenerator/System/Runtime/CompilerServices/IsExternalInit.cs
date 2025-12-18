namespace System.Runtime.CompilerServices;
[LangVersion(FutureCSharpLanguageVersion.CSharp9)]
#if !NET5_0_OR_GREATER
[ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
internal static class IsExternalInit { }
#endif