namespace System;

internal sealed class AliasAttribute : Attribute
{
    public string Name { get; set; }
    public AliasAttribute(string name)
    {
        Name = name;
    }
}
