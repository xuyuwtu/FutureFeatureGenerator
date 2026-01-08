#region
#endregion
namespace System.Collections.Generic;

internal static partial class FutureList
{
    #region EnsureCapacity(int)
    // System.Array.MaxLength
    [Alias(nameof(EnsureCapacity))]
#if !NET6_0_OR_GREATER
    internal static int EnsureCapacity<T>(this List<T> self, int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");
        }
        if (self.Capacity < capacity)
        {
            int newCapacity = self.Capacity == 0 ? 4 : 2 * self.Capacity;
            if ((uint)newCapacity > FutureArray.MaxLength)
            {
                newCapacity = FutureArray.MaxLength;
            }
            if (newCapacity < capacity)
            {
                newCapacity = capacity;
            }
            self.Capacity = newCapacity;
        }
        return self.Capacity;
    }
#endif
    #endregion
}
