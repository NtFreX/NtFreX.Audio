﻿using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class WaveEnumerableAudioContainerExtensions
    {
        [return: NotNull]
        public static async Task<WaveStreamAudioContainer> ToInMemoryContainerAsync([NotNull] this WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            return await audio.ToStreamAsync(new MemoryStream(), cancellationToken).ConfigureAwait(false);
        }

        [return: NotNull]
        public static WaveEnumerableAudioContainer LogProgress([NotNull] this WaveEnumerableAudioContainer audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var modifier = (double)audio.DataSubChunk.ChunkSize / (audio.FmtSubChunk.BitsPerSample / 8);
            return audio.WithDataSubChunk(x => x.WithData(audio.DataSubChunk.Data.ForEachAsync((index, _) => onProgress.Invoke(index / modifier), cancellationToken)));
        }

        [return: NotNull]
        public static async Task<WaveStreamAudioContainer> ToFileAsync([NotNull] this WaveEnumerableAudioContainer audio, [NotNull] string path, [NotNull] FileMode fileMode = FileMode.CreateNew, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            return await audio.ToFileAsync(path, fileMode, cancellationToken).ConfigureAwait(false);
        }
    }
}
