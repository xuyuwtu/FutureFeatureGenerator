#region
#endregion
namespace System.IO;
internal static partial class FutureStream
{
    // System.Span`1
    #region Read(Span<byte>)
    [Alias(nameof(Read))]
#if !NETCOREAPP2_1_OR_GREATER
    internal static int Read(this Stream self, Span<byte> buffer)
    {
        byte[] array = Buffers.ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            int num = self.Read(array, 0, buffer.Length);
            if ((uint)num > (uint)buffer.Length)
            {
                throw new IOException("Stream was too long.");
            }
            new ReadOnlySpan<byte>(array, 0, num).CopyTo(buffer);
            return num;
        }
        finally
        {
            Buffers.ArrayPool<byte>.Shared.Return(array);
        }
    }
#endif
    #endregion

    // System.ReadOnlySpan`1
    #region Write(ReadOnlySpan<byte>)
    [Alias(nameof(Write))]
#if !NETCOREAPP2_1_OR_GREATER
    internal static void Write(this Stream self, ReadOnlySpan<byte> buffer)
    {
        byte[] array = Buffers.ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            buffer.CopyTo(array);
            self.Write(array, 0, buffer.Length);
        }
        finally
        {
            Buffers.ArrayPool<byte>.Shared.Return(array);
        }
    }
#endif
    #endregion
}