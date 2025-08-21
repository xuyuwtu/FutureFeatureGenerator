using System.Diagnostics;

namespace System;
[AttributeUsage(AttributeTargets.All)]
[Conditional("FALSE")]
internal class RealConditionAttribute : Attribute
{
    public string Condition { get; }

    public RealConditionAttribute(string condition)
    {
        Condition = condition;
    }
}
