using NtFreX.Audio.Math;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    internal static class StreamExtensions
    {
        public static async Task<Stream> SkipAsync(this Stream stream, int length, CancellationToken cancellationToken = default)
        {
            if (stream.CanSeek)
            {
                stream.Seek(length, SeekOrigin.Current);
            }
            else
            {
                await stream.ReadAsync(new byte[length], 0, length, cancellationToken).ConfigureAwait(false);
            }
            return stream;
        }

        public static async Task<int> ReadInt32Async(this Stream stream, bool isLittleEndian = true, CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(4, cancellationToken).ConfigureAwait(false);
            return buffer.ToInt32(isLittleEndian);
        }

        public static async Task<uint> ReadUInt32Async(this Stream stream, bool isLittleEndian = true, CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(4, cancellationToken).ConfigureAwait(false);
            return buffer.ToUInt32(isLittleEndian);
        }

        public static async Task<short> ReadInt16Async(this Stream stream, bool isLittleEndian = true, CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(2, cancellationToken).ConfigureAwait(false);
            return buffer.ToInt16(isLittleEndian);
        }

        public static async Task<ushort> ReadUInt16Async(this Stream stream, bool isLittleEndian = true, CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(2, cancellationToken).ConfigureAwait(false);
            return buffer.ToUInt16(isLittleEndian);
        }

        public static async Task<byte[]> ReadBytesAsync(this Stream stream, int requiredLength, CancellationToken cancellationToken = default)
        {
            var buffer = new byte[requiredLength];
            var length = await stream.ReadAsync(buffer, 0, requiredLength, cancellationToken).ConfigureAwait(false);
            if (length != requiredLength)
            {
                throw new InvalidOperationException("The given stream doesn't contain the 4 bytes nessesary to create an integer");
            }
            return buffer;
        }
    }
}
