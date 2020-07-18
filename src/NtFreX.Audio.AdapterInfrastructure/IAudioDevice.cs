using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.AdapterInfrastructure
{
    //TODO: change interfaces and work with one format? (make it possible to interact without wave audioContainer)
    public sealed class Format
    {
        public uint SampleRate { get; }
        public ushort BitsPerSample { get; }

        public Format(uint sampleRate, ushort bitPerSample)
        {
            SampleRate = sampleRate;
            BitsPerSample = bitPerSample;
        }
    }

    public interface IAudioDevice : IDisposable
    {
        bool IsInitialized();
        bool TryInitialize(IWaveAudioContainer audio, out Format supportedFormat);

        [return: NotNull] Task<IPlaybackContext> PlayAsync(CancellationToken cancellationToken = default);
    }
}
