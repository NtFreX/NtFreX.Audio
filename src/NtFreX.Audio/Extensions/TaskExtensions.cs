using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<IntermediateEnumerableAudioContainer> LogProgressAsync(this Task<IntermediateEnumerableAudioContainer> audio, Action<double> onProgress, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var data = await audio.ConfigureAwait(false);
            return data.LogProgress(onProgress, cancellationToken);
        }
    }
}
