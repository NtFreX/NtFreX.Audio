using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal interface IAudioContainerSerializer
    {
        string PreferredFileExtension { get; }

        Task ToFileAsync(string path, AudioContainer container, CancellationToken cancellationToken = default);
        Task<AudioContainer> FromFileAsync(string path, CancellationToken cancellationToken = default);

        Task<byte[]> ToDataAsync(AudioContainer container, CancellationToken cancellationToken = default);
        Task<AudioContainer> FromDataAsync(byte[] data, CancellationToken cancellationToken = default);

        Task ToStreamAsync(AudioContainer container, Stream stream, CancellationToken cancellationToken = default);
        Task<AudioContainer> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default);
    }
}
