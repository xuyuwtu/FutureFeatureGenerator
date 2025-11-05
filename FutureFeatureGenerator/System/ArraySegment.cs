#region
#endregion
namespace System;

internal static partial class FutureArraySegment
{
    #region CopyTo(T[])
    // System.ArraySegment.CopyTo(T[],int)
    [Alias(nameof(CopyTo))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static void CopyTo<T>(this ArraySegment<T> self, T[] destination)
    {
        CopyTo(self, destination, 0);
    }
#endif
    #endregion

    #region CopyTo(T[],int)
    [Alias(nameof(CopyTo))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static void CopyTo<T>(this ArraySegment<T> self, T[] destination, int destinationIndex)
    {
        if (self.Array == null)
        {
            throw new InvalidOperationException("The underlying array is null.");
        }
        Array.Copy(self.Array, self.Offset, destination, destinationIndex, self.Count);
    }
#endif
    #endregion

    #region CopyTo(ArraySegment<T>)
    [Alias(nameof(CopyTo))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static void CopyTo<T>(this ArraySegment<T> self, ArraySegment<T> destination)
    {
        if (self.Array == null)
        {
            throw new InvalidOperationException("The underlying array is null.");
        }
        if (destination.Array == null)
        {
            throw new InvalidOperationException("The underlying array is null.");
        }
        if (self.Count > destination.Count)
        {
            throw new ArgumentException("Destination is too short.", nameof(destination));
        }
        Array.Copy(self.Array, self.Offset, destination.Array, destination.Offset, self.Count);
    }
#endif
    #endregion

    #region Slice(int)
    [Alias(nameof(Slice))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static ArraySegment<T> Slice<T>(this ArraySegment<T> self, int index)
    {
        if (self.Array == null)
        {
            throw new InvalidOperationException("The underlying array is null.");
        }
        if ((uint)index > (uint)self.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        return new ArraySegment<T>(self.Array, self.Offset + index, self.Count - index);
    }
#endif
    #endregion

    #region Slice(int,int)
    [Alias(nameof(Slice))]
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static ArraySegment<T> Slice<T>(this ArraySegment<T> self, int index, int count)
    {
        if (self.Array == null)
        {
            throw new InvalidOperationException("The underlying array is null.");
        }
        if ((uint)index > (uint)self.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }
        return new ArraySegment<T>(self.Array, self.Offset + index, count);
    }
#endif
    #endregion

    #region ToArray()
#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
    internal static T[] ToArray<T>(this ArraySegment<T> self)
    {
        if (self.Array == null)
        {
            throw new InvalidOperationException("The underlying array is null.");
        }
        if (self.Count == 0)
        {
            //return ArraySegment<T>.Empty.Array;
            return Array.Empty<T>();
        }
        T[] array = new T[self.Count];
        Array.Copy(self.Array, self.Offset, array, 0, self.Count);
        return array;
    }
#endif
    #endregion
}
