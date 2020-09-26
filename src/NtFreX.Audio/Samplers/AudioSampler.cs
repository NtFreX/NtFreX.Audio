using NtFreX.Audio.Containers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public abstract class AudioSampler
    {
        public Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateListAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            return SampleAsync(audio.AsEnumerable(), cancellationToken);
        }

        public abstract Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default);
    }
}
