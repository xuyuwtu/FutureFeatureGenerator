namespace System.Runtime.CompilerServices;
#if !NET7_0_OR_GREATER
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
[ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
internal sealed class RequiredMemberAttribute : Attribute { }
#endif