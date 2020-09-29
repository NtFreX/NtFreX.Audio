using NtFreX.Audio.Infrastructure.Container;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    public sealed class AudioContainerSerializerFactory
    {
        private readonly IAudioContainerSerializer[] audioContainerSerializers = new IAudioContainerSerializer[] {
            new WaveAudioContainerSerializer(),
            new IntermediateAudioContainerSerializer()
        };

        public static AudioContainerSerializerFactory Instance { get; } = new AudioContainerSerializerFactory();

        private AudioContainerSerializerFactory() { }

        public Task<IAudioContainer> FromFileAsync(string path, CancellationToken cancellationToken = default) => FromFileAsync(path, Path.GetExtension(path), cancellationToken);
        public Task<IAudioContainer> FromFileAsync(string path, string extension, CancellationToken cancellationToken = default) => ForExtension(extension).FromFileAsync(path, cancellationToken);
        public Task<IAudioContainer> FromDataAsync(byte[] data, string extension, CancellationToken cancellationToken = default) => ForExtension(extension).FromDataAsync(data, cancellationToken);
        public Task<IAudioContainer> FromStreamAsync(Stream stream, string extension, CancellationToken cancellationToken = default) => ForExtension(extension).FromStreamAsync(stream, cancellationToken);

        public Task ToStreamAsync(IAudioContainer container, Stream stream, CancellationToken cancellationToken = default) => ForContainer(container).ToStreamAsync(container, stream, cancellationToken);
        public Task ToFileAsync(string path, IAudioContainer container, CancellationToken cancellationToken = default) => ForContainer(container).ToFileAsync(path, container, cancellationToken);
        public Task<byte[]> ToDataAsync(IAudioContainer container, CancellationToken cancellationToken = default) => ForContainer(container).ToDataAsync(container, cancellationToken);
        public string GetPreferredFileExtension(IAudioContainer container) => ForContainer(container).PreferredFileExtension;

        private IAudioContainerSerializer ForExtension(string extension) => audioContainerSerializers.First(x => x.PreferredFileExtension == extension);
        private IAudioContainerSerializer ForContainer(IAudioContainer container) => audioContainerSerializers.First(x => x.GetType().BaseType?.GenericTypeArguments.First().IsAssignableFrom(container.GetType()) ?? false);
    }
}
