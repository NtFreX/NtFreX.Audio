using NtFreX.Audio.Infrastructure.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal abstract class AudioContainerSerializer
    {
        public static async Task WriteDataAsync(ISeekableAsyncEnumerable<IReadOnlyList<byte>> data, Stream stream, CancellationToken cancellationToken = default)
        {
            await using var enumerator = data.GetAsyncEnumerator(cancellationToken);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                await stream.WriteAsync(enumerator.Current.ToArray(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
