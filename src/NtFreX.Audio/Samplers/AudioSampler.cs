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

#pragma warning disable CA2000 // Dispose objects before losing scope => object is wrapped by another object which cleans this up
            return SampleAsync(audio.AsEnumerable(), cancellationToken);
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        public abstract Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default);
    }
}
