using NtFreX.Audio.Containers;
using NtFreX.Audio.Containers.Serializers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class WaveAudioContainerStream
    {
        public WaveAudioContainer Container { [return: NotNull] get; }
        public IAsyncEnumerable<byte[]> Stream { [return: NotNull] get; }

        public WaveAudioContainerStream([NotNull] WaveAudioContainer container, [NotNull] IAsyncEnumerable<byte[]> stream)
        {
            Container = container;
            Stream = stream;
        }

        [return: NotNull]
        public static WaveAudioContainerStream ToStream([NotNull] WaveAudioContainer container, CancellationToken cancellationToken = default)
            => new WaveAudioContainerStream(container, container.GetAudioSamplesAsync(cancellationToken));

        [return: NotNull]
        public Task<WaveAudioContainer> ToContainerAsync([NotNull] string path, [NotNull] FileMode fileMode = FileMode.CreateNew, [MaybeNull] CancellationToken cancellationToken = default)
            => ToContainerAsync(new FileStream(path, fileMode, FileAccess.ReadWrite), cancellationToken);

        [return: NotNull]
        public async Task<WaveAudioContainer> ToContainerAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var serializer = AudioContainerSerializerFactory.WaveAudioContainerSerializer;
            await serializer.WriteHeadersAsync(Container, stream, cancellationToken).ConfigureAwait(false);
            await serializer.WriteDataAsync(Stream, stream, cancellationToken).ConfigureAwait(false);

            return Container.WithDataSubChunk(x => x.WithData(stream));
        }
    }
}
