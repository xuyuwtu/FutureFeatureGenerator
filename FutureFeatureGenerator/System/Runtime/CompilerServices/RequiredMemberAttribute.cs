namespace System.Runtime.CompilerServices;
/// <see cref="CSharpFeatureNames.None"/>
#if !NET7_0_OR_GREATER
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute { }
#endif