using NtFreX.Audio.Infrastructure.Threading;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal abstract class AudioContainerSerializer
    {
        public static async Task WriteDataAsync(ISeekableAsyncEnumerable<Memory<byte>> data, Stream stream, CancellationToken cancellationToken = default)
        {
            await using var enumerator = data.GetAsyncEnumerator(cancellationToken);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                await stream.WriteAsync(enumerator.Current.ToArray(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
