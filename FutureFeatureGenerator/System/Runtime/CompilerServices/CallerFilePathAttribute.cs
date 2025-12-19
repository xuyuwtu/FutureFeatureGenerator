namespace System.Runtime.CompilerServices;
#if !NETCOREAPP && !NETSTANDARD && !NET45_OR_GREATER
[LangVersion(FutureCSharpLanguageVersion.CSharp5)]
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class CallerFilePathAttribute : Attribute { }
#endif