using System;

namespace NtFreX.Audio.Infrastructure
{
    public interface IStreamAudioContainer : IAudioContainer, IDisposable
    {
        //[return: NotNull] public Task ToFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default) => AudioEnvironment.Serializer.ToFileAsync(path, this, cancellationToken);
    }
}
