using System.Linq;

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
        public AudioContainer FromFile(string path) => _audioContainerSerializers.First().FromFile(path);
        public AudioContainer FromData(byte[] data) => _audioContainerSerializers.First().FromData(data);

        public void ToFile(string path, AudioContainer container) => ForContainer(container).ToFile(path, container);
        public byte[] ToData(AudioContainer container) => ForContainer(container).ToData(container);
        public string GetPreferredFileExtension(AudioContainer container) => ForContainer(container).PreferredFileExtension;


        private IAudioContainerSerializer ForContainer(AudioContainer container) => _audioContainerSerializers.First(x => x.GetType().BaseType.GenericTypeArguments.First() == container.GetType());
    }
}
