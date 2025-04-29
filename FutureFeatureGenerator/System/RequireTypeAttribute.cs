namespace System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class RequireTypeAttribute : Attribute
{
    public string TypeFullName { get; }

    public RequireTypeAttribute(string typeFullName)
    {
        TypeFullName = typeFullName;
    }
}
