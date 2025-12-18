using System.Diagnostics;

namespace System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
[Conditional("FALSE")]
internal sealed class LangVersion : Attribute
{
    public LangVersion(FutureCSharpLanguageVersion version)
    {
    }
}
