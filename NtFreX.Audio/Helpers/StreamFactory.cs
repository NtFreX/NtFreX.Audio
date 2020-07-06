using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Helpers
{
    //TODO: use factory everywhere for buffersize and stream resolver
    internal sealed class StreamFactory
    {
        public static StreamFactory Instance { [return:NotNull] get; } = new StreamFactory();

        [return: NotNull]
        public async Task<Stream> WriteToNewStreamAsync([NotNull] byte[] values, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var stream = ResolveNewStream();
            await stream.WriteAsync(values, cancellationToken).ConfigureAwait(false);
            return stream;
        }

        [return: NotNull]
        public async Task<Stream> WriteToNewStreamAsync([NotNull] IAsyncEnumerable<byte> values, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var stream = ResolveNewStream();
            var buffer = new byte[GetBufferSize()];
            var bufferIndex = 0;
            var totalIndex = 0;
            await foreach (var value in values.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                buffer[bufferIndex++] = value;

                if (bufferIndex == buffer.Length)
                {
                    await stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
                    bufferIndex = 0;
                }
                
                onProgress(totalIndex++);
            }

            if (bufferIndex > 0)
            {
                await stream.WriteAsync(buffer, 0, bufferIndex, cancellationToken).ConfigureAwait(false);
            }

            return stream;
        }

        public int GetBufferSize() => 2000;

        [return: NotNull] public Stream ResolveNewStream() => new MemoryStream();
        [return: NotNull] public Stream ResolveNewStreamForInMemory() => new MemoryStream();
    }
}
