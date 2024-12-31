namespace System.Runtime.CompilerServices;
// 5
#if (!NETCOREAPP && !NETSTANDARD && !NET45_OR_GREATER)
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class CallerMemberNameAttribute : Attribute { }
#endif