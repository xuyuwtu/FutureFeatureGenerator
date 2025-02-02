namespace System.Runtime.CompilerServices;
/// <see cref="CSharpFeatureNames.None"/>
#if (!NETCOREAPP && !NETSTANDARD && !NET45_OR_GREATER)
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class CallerLineNumberAttribute : Attribute { }
#endif