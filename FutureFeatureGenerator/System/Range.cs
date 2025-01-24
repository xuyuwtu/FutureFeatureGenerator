namespace System;
// 8.0
// System.Index
// System.Diagnostics.CodeAnalysis.NotNullWhenAttribute
#if !(NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
internal readonly struct Range : IEquatable<Range>
{
    public Index Start { get; }
    public Index End { get; }
    public Range(Index start, Index end)
    {
        Start = start;
        End = end;
    }
    public override bool Equals([Diagnostics.CodeAnalysis.NotNullWhen(true)] object? value)
    {
        return value is Range r && r.Start.Equals(Start) && r.End.Equals(End);
    }
    public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);
    public override int GetHashCode()
    {
        int h1 = Start.GetHashCode();
        int h2 = End.GetHashCode();
        uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
        return ((int)rol5 + h1) ^ h2;
    }
    public override string ToString()
    {
        return Start.ToString() + ".." + End.ToString();
    }
    public static Range StartAt(Index start) => new Range(start, Index.End);
    public static Range EndAt(Index end) => new Range(Index.Start, end);
    public static Range All => new Range(Index.Start, Index.End);
    #if (NETCOREAPP || NETSTANDARD || NET45_OR_GREATER)
    [Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    #endif
    public (int Offset, int Length) GetOffsetAndLength(int length)
    {
        int start = Start.GetOffset(length);
        int end = End.GetOffset(length);

        if ((uint)end > (uint)length || (uint)start > (uint)end)
        {
            ThrowArgumentOutOfRangeException();
        }

        return (start, end - start);
    }
    private static void ThrowArgumentOutOfRangeException()
    {
        throw new ArgumentOutOfRangeException("length");
    }
}
#endif