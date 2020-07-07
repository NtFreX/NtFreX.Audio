using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    public interface IAudioContainerSerializer
    {
        string PreferredFileExtension { [return: NotNull] get; }

        [return: NotNull] Task ToFileAsync([NotNull] string path, [NotNull] AudioContainer container, [MaybeNull] CancellationToken cancellationToken = default);
        [return: NotNull] Task<AudioContainer> FromFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default);

        [return: NotNull] Task<byte[]> ToDataAsync([NotNull] AudioContainer container, [MaybeNull] CancellationToken cancellationToken = default);
        [return: NotNull] Task<AudioContainer> FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken = default);

        [return: NotNull] Task ToStreamAsync([NotNull] AudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);
        [return: NotNull] Task<AudioContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);
    }
}
