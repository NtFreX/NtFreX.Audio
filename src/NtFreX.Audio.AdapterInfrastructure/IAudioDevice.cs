using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioDevice : IDisposable
    {
        bool IsInitialized();
        bool TryInitialize(IWaveAudioContainer audio, out Format supportedFormat);

        [return: NotNull] Task<IPlaybackContext> PlayAsync(CancellationToken cancellationToken = default);
    }
}
