using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class VolumeAudioSampler : AudioSampler 
    {
        private readonly double volumeFactor;

        public VolumeAudioSampler(double volumeFactor)
        {
            this.volumeFactor = volumeFactor;
        }

        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var max = (long)System.Math.Pow(256, audio.GetFormat().BitsPerSample / 8) / 2;
            var min = max * -1;

            //TODO: smooth normalize when reaching max signal?
            return Task.FromResult(audio.WithData(
                data: audio.SelectAsync(x => x * volumeFactor, cancellationToken)));
        }

        public override string ToString()
        {
            return base.ToString() + $", volumeFactor={volumeFactor}";
        }
    }
}
