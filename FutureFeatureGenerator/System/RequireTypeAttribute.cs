using System.Diagnostics;

namespace System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
[Conditional("FALSE")]
internal sealed class RequireType : Attribute
{
    public string TypeFullName { get; }

    public RequireType(string typeFullName)
    {
        TypeFullName = typeFullName;
    }
}
