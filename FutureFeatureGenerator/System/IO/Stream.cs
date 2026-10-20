#region
#endregion
namespace System.IO;
internal static partial class FutureStream
{
    // System.Span`1
    #region Read(Span<byte>)
    [Alias(nameof(Read))]
#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
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
#if !(NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
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

    // System.Span`1
    // System.IO.Stream.Read(Span<byte>)
    #region ReadExactly(Span<byte>)
    [Alias(nameof(ReadExactly))]
#if !NET7_0_OR_GREATER
    internal static void ReadExactly(this Stream self, Span<byte> buffer)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = self.Read(buffer.Slice(totalRead));
            if (read == 0)
            {
                throw new EndOfStreamException();
            }
            totalRead += read;
        }
    }
#endif
    #endregion

    #region ReadExactly(byte[],int,int)
#if !NET7_0_OR_GREATER
    internal static void ReadExactly(this Stream self, byte[] buffer, int offset, int count)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = self.Read(buffer, offset + totalRead, count - totalRead);
            if (read == 0)
            {
                throw new EndOfStreamException();
            }
            totalRead += read;
        }
    }
#endif
    #endregion
}