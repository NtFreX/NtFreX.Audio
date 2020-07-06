using NtFreX.Audio.Math;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class StreamExtensions
    {
        [return:NotNull] public static async Task<Stream> SkipAsync([NotNull] this Stream stream, int length, [MaybeNull] CancellationToken cancellationToken = default)
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

        [return: NotNull] public static async Task<string> ReadStringAsync([NotNull] this Stream stream, int length, bool isLittleEndian = true, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(length, cancellationToken).ConfigureAwait(false);
            return buffer.ToAscii(isLittleEndian);
        }

        [return: NotNull] public static async Task<int> ReadInt32Async([NotNull] this Stream stream, bool isLittleEndian = true, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(4, cancellationToken).ConfigureAwait(false);
            return buffer.ToInt32(isLittleEndian);
        }

        [return: NotNull] public static async Task<uint> ReadUInt32Async([NotNull] this Stream stream, bool isLittleEndian = true, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(4, cancellationToken).ConfigureAwait(false);
            return buffer.ToUInt32(isLittleEndian);
        }

        [return: NotNull] public static async Task<short> ReadInt16Async([NotNull] this Stream stream, bool isLittleEndian = true, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(2, cancellationToken).ConfigureAwait(false);
            return buffer.ToInt16(isLittleEndian);
        }

        [return: NotNull] public static async Task<ushort> ReadUInt16Async([NotNull] this Stream stream, bool isLittleEndian = true, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var buffer = await stream.ReadBytesAsync(2, cancellationToken).ConfigureAwait(false);
            return buffer.ToUInt16(isLittleEndian);
        }

        [return: NotNull] public static async Task<byte[]> ReadBytesAsync([NotNull] this Stream stream, int requiredLength, [MaybeNull] CancellationToken cancellationToken = default)
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
