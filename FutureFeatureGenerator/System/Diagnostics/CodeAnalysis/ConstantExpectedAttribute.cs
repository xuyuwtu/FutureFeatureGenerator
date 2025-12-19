namespace System.Diagnostics.CodeAnalysis;

#if !NET7_0_OR_GREATER
internal sealed class ConstantExpectedAttribute : Attribute
{
    public object? Min { get; set; }
    public object? Max { get; set; }
}
#endif