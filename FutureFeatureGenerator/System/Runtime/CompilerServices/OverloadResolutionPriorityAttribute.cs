namespace System.Runtime.CompilerServices;
// 13.0
#if !NET9_0_OR_GREATER
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class OverloadResolutionPriorityAttribute : Attribute
{
    public int Priority { get; }
    public OverloadResolutionPriorityAttribute(int priority)
    {
        Priority = priority;
    }
}
#endif