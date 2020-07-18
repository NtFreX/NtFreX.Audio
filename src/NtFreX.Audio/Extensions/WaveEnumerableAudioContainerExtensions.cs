using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace NtFreX.Audio.Extensions
{
    public static class WaveEnumerableAudioContainerExtensions
    {
        [return: NotNull]
        public static WaveEnumerableAudioContainer LogProgress([NotNull] this WaveEnumerableAudioContainer audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var modifier = (double)audio.DataSubChunk.Subchunk2Size / (audio.FmtSubChunk.BitsPerSample / 8);
            return audio.WithDataSubChunk(x => x.WithData(audio.DataSubChunk.Data.ForEachAsync((index, _) => onProgress.Invoke(index / modifier), cancellationToken)));
        }
    }
}
