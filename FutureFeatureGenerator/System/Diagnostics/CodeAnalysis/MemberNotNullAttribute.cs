// 9.0
#if !NET5_0_OR_GREATER
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullAttribute : Attribute
{
    public string[] Members { get; }
    public MemberNotNullAttribute(string member)
    {
        Members = new string[] { member };
    }
    public MemberNotNullAttribute(params string[] members)
    {
        Members = members;
    }
}
#endif