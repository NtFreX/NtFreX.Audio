using NtFreX.Audio.Infrastructure.Container;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    public sealed class AudioContainerSerializerFactory
    {
        private readonly IAudioContainerSerializer[] audioContainerSerializers = new IAudioContainerSerializer[] {
            new WaveAudioContainerSerializer()
        };

        public static AudioContainerSerializerFactory Instance { [return:NotNull] get; } = new AudioContainerSerializerFactory();

        private AudioContainerSerializerFactory() { }

        [return:NotNull] public Task<IStreamAudioContainer> FromFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default) => audioContainerSerializers.First().FromFileAsync(path, cancellationToken);
        [return:NotNull] public Task<IStreamAudioContainer> FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken = default) => audioContainerSerializers.First().FromDataAsync(data, cancellationToken);
        [return:NotNull] public Task<IStreamAudioContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default) => audioContainerSerializers.First().FromStreamAsync(stream, cancellationToken);

        [return: NotNull] public Task ToStreamSync([NotNull] IStreamAudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default) => audioContainerSerializers.First().ToStreamAsync(container, stream, cancellationToken);
        [return: NotNull] public Task ToFileAsync([NotNull] string path, [NotNull] IStreamAudioContainer container, [MaybeNull] CancellationToken cancellationToken = default) => ForContainer(container).ToFileAsync(path, container, cancellationToken);
        [return: NotNull] public Task<byte[]> ToDataAsync([NotNull] IStreamAudioContainer container, [MaybeNull] CancellationToken cancellationToken = default) => ForContainer(container).ToDataAsync(container, cancellationToken);
        [return: NotNull] public string GetPreferredFileExtension([NotNull] IStreamAudioContainer container) => ForContainer(container).PreferredFileExtension;

        [return: NotNull] private IAudioContainerSerializer ForContainer([NotNull] IStreamAudioContainer container) => audioContainerSerializers.First(x => x.GetType().BaseType?.GenericTypeArguments.First() == container.GetType());
    }
}
