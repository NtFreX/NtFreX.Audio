using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioDevice : IDisposable
    {
        [return: NotNull] Task<IPlaybackContext> PlayAsync([NotNull] IWaveAudioContainer audio, CancellationToken cancellationToken = default);
    }
}
