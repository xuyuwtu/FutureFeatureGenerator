namespace System.Runtime.CompilerServices;
#if !NET7_0_OR_GREATER
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public const string RefStructs = nameof(RefStructs);

    public const string RequiredMembers = nameof(RequiredMembers);
    public string FeatureName { get; }
    public bool IsOptional { get; init; }
    public CompilerFeatureRequiredAttribute(string featureName)
    {
        FeatureName = featureName;
    }
}
#endif