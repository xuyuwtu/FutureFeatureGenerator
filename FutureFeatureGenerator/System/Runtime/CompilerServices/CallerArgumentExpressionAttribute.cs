﻿namespace System.Runtime.CompilerServices;
// 10.0
#if !NET6_0_OR_GREATER
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
    public string ParameterName { get; }
    public CallerArgumentExpressionAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }
}
#endif