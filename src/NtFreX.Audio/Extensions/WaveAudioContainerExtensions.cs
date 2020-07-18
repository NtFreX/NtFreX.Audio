using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
using System.Threading;

namespace NtFreX.Audio.Extensions
{
    public static class WaveAudioContainerExtensions
    {
        public static WaveEnumerableAudioContainer AsEnumerable(this IWaveAudioContainer audio, CancellationToken cancellationToken = default)
        {
            //TODO: is this clean code?
            if (audio is WaveEnumerableAudioContainer waveEnumerableAudioContainer)
            {
                return waveEnumerableAudioContainer;
            }
            if (audio is WaveStreamAudioContainer waveStreamAudioContainer)
            {
                return WaveEnumerableAudioContainer.ToEnumerable(waveStreamAudioContainer, cancellationToken);
            }

            throw new ArgumentException("The given wave container type is not supported", nameof(audio));
        }
    }
}
