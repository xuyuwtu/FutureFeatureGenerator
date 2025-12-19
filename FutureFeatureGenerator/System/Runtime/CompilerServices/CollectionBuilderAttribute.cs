namespace System.Runtime.CompilerServices;

[LangVersion(FutureCSharpLanguageVersion.CSharp12)]
#if !NET8_0_OR_GREATER
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
internal sealed class CollectionBuilderAttribute : Attribute
{
    public Type BuilderType { get; }
    public string MethodName { get; }
    public CollectionBuilderAttribute(Type builderType, string methodName)
    {
        BuilderType = builderType;
        MethodName = methodName;
    }
}
#endif