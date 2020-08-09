using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// If the stream supports seeking this method will seek by the given length from the current position.
        /// Else the stream is read by the given length and the data is discarded
        /// </summary>
        /// <param name="stream">The stream</param>
        /// <param name="length">The length to skip from the current position</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The given stream</returns>
        [return:NotNull] public static async Task<Stream> SkipAsync([NotNull] this Stream stream, int length, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (stream.CanSeek)
            {
                stream.Seek(length, SeekOrigin.Current);
            }
            else
            {
                _ = await stream.ReadAsync(new byte[length], 0, length, cancellationToken).ConfigureAwait(false);
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
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            var buffer = new byte[requiredLength];
            var length = await stream.ReadAsync(buffer, 0, requiredLength, cancellationToken).ConfigureAwait(false);
            if (length != requiredLength)
            {
                throw new InvalidOperationException(ExceptionMessages.StreamToShort);
            }
            return buffer;
        }
    }
}
