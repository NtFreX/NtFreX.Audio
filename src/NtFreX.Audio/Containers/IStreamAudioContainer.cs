using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public interface IStreamAudioContainer : IAudioContainer, IDisposable
    {
        [return: NotNull] public Task ToFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default) => AudioEnvironment.Serializer.ToFileAsync(path, this, cancellationToken);
    }
}
