using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class SampleRateAudioSampler : AudioSampler
    {
        private readonly uint sampleRate;

        public SampleRateAudioSampler(uint sampleRate)
        {
            this.sampleRate = sampleRate;
        }

        [return:NotNull] public override Task<WaveAudioContainer> SampleAsync([NotNull] WaveAudioContainer audio, [NotNull] Action<double> onProgress, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (audio.FmtSubChunk.SampleRate == sampleRate)
            {
                return Task.FromResult(audio);
            }

            // TODO: implement
            throw new NotImplementedException();
        }
    }
}
