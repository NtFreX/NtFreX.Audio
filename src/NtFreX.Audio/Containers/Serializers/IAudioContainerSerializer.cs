using NtFreX.Audio.Infrastructure.Container;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal interface IAudioContainerSerializer
    {
        string PreferredFileExtension { get; }

        Task ToFileAsync(string path, IAudioContainer container, CancellationToken cancellationToken = default);
        Task<IAudioContainer> FromFileAsync(string path, CancellationToken cancellationToken = default);

        Task<byte[]> ToDataAsync(IAudioContainer container, CancellationToken cancellationToken = default);
        Task<IAudioContainer> FromDataAsync(byte[] data, CancellationToken cancellationToken = default);

        Task ToStreamAsync(IAudioContainer container, Stream stream, CancellationToken cancellationToken = default);
        Task<IAudioContainer> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default);
    }
}
