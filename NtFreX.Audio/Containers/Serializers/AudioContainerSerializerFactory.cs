using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    public sealed class AudioContainerSerializerFactory
    {
        private readonly IAudioContainerSerializer[] _audioContainerSerializers = new IAudioContainerSerializer[] {
            new WaveAudioContainerSerializer()
        };

        public static AudioContainerSerializerFactory Instance { get; } = new AudioContainerSerializerFactory();

        private AudioContainerSerializerFactory() { }

        // TODO: resolve by signature in file or/and by file extension
        public Task<AudioContainer> FromFileAsync(string path, CancellationToken cancellationToken = default) => _audioContainerSerializers.First().FromFileAsync(path, cancellationToken);
        public Task<AudioContainer> FromDataAsync(byte[] data, CancellationToken cancellationToken = default) => _audioContainerSerializers.First().FromDataAsync(data, cancellationToken);
        public Task<AudioContainer> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default) => _audioContainerSerializers.First().FromStreamAsync(stream, cancellationToken);

        public Task ToStreamSync(AudioContainer container, Stream stream, CancellationToken cancellationToken = default) => _audioContainerSerializers.First().ToStreamAsync(container, stream, cancellationToken);
        public Task ToFileAsync(string path, AudioContainer container, CancellationToken cancellationToken = default) => ForContainer(container).ToFileAsync(path, container, cancellationToken);
        public Task<byte[]> ToDataAsync(AudioContainer container, CancellationToken cancellationToken = default) => ForContainer(container).ToDataAsync(container, cancellationToken);
        public string GetPreferredFileExtension(AudioContainer container) => ForContainer(container).PreferredFileExtension;

        private IAudioContainerSerializer ForContainer(AudioContainer container) => _audioContainerSerializers.First(x => x.GetType().BaseType.GenericTypeArguments.First() == container.GetType());
    }
}
