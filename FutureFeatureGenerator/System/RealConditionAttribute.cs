namespace System;

internal class RealConditionAttribute : Attribute
{
    public string Condition { get; }

    public RealConditionAttribute(string condition)
    {
        Condition = condition;
    }
}
