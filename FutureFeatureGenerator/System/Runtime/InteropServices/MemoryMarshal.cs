#region
#endregion
namespace System.Runtime.InteropServices;

internal static partial class FutureMemoryMarshal
{
    #region GetArrayDataReference(T[])
    [RequireType(nameof(System.Runtime.CompilerServices.RawArrayData))]
#if !NET5_0_OR_GREATER
#if NETCOREAPP || NETSTANDARD || NET45_OR_GREATER
    [CompilerServices.MethodImpl(CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
    internal static ref T GetArrayDataReference<T>(T[] array)
    {
        return ref CompilerServices.Unsafe.As<byte, T>(ref CompilerServices.Unsafe.As<CompilerServices.RawArrayData>(array).Data);
    }
#endif
    #endregion
}
