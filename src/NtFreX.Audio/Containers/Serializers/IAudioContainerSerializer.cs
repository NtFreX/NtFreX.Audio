using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal interface IAudioContainerSerializer
    {
        string PreferredFileExtension { [return: NotNull] get; }

        [return: NotNull] Task ToFileAsync([NotNull] string path, [NotNull] IStreamAudioContainer container, [MaybeNull] CancellationToken cancellationToken = default);
        [return: NotNull] Task<IStreamAudioContainer> FromFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default);

        [return: NotNull] Task<byte[]> ToDataAsync([NotNull] IStreamAudioContainer container, [MaybeNull] CancellationToken cancellationToken = default);
        [return: NotNull] Task<IStreamAudioContainer> FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken = default);

        [return: NotNull] Task ToStreamAsync([NotNull] IStreamAudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);
        [return: NotNull] Task<IStreamAudioContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);
    }
}
