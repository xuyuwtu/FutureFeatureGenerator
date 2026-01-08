#region
#endregion
namespace System;

internal static partial class FutureArray
{
    #region MaxLength
#if !NET6_0_OR_GREATER
    internal static int MaxLength => 0X7FFFFFC7;
#endif
    #endregion
}
