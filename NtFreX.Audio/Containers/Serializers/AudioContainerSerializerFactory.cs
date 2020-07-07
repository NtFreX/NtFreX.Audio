using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    public sealed class AudioContainerSerializerFactory
    {
        [return: NotNull] public static WaveAudioContainerSerializer WaveAudioContainerSerializer => new WaveAudioContainerSerializer();

        private readonly IAudioContainerSerializer[] _audioContainerSerializers = new IAudioContainerSerializer[] {
            WaveAudioContainerSerializer
        };

        public static AudioContainerSerializerFactory Instance { [return:NotNull] get; } = new AudioContainerSerializerFactory();

        private AudioContainerSerializerFactory() { }

        // TODO: resolve by signature in file or/and by file extension
        [return:NotNull] public Task<AudioContainer> FromFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default) => _audioContainerSerializers.First().FromFileAsync(path, cancellationToken);
        [return:NotNull] public Task<AudioContainer> FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken = default) => _audioContainerSerializers.First().FromDataAsync(data, cancellationToken);
        [return:NotNull] public Task<AudioContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default) => _audioContainerSerializers.First().FromStreamAsync(stream, cancellationToken);

        [return: NotNull] public Task ToStreamSync([NotNull] AudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default) => _audioContainerSerializers.First().ToStreamAsync(container, stream, cancellationToken);
        [return: NotNull] public Task ToFileAsync([NotNull] string path, [NotNull] AudioContainer container, [MaybeNull] CancellationToken cancellationToken = default) => ForContainer(container).ToFileAsync(path, container, cancellationToken);
        [return: NotNull] public Task<byte[]> ToDataAsync([NotNull] AudioContainer container, [MaybeNull] CancellationToken cancellationToken = default) => ForContainer(container).ToDataAsync(container, cancellationToken);
        [return: NotNull] public string GetPreferredFileExtension([NotNull] AudioContainer container) => ForContainer(container).PreferredFileExtension;

        [return: NotNull] private IAudioContainerSerializer ForContainer([NotNull] AudioContainer container) => _audioContainerSerializers.First(x => x.GetType().BaseType?.GenericTypeArguments.First() == container.GetType());
    }
}
