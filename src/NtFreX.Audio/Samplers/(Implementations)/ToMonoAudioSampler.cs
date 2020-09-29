using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    /// <summary>
    /// converts stereo to mono 
    /// </summary>
    public class ToMonoAudioSampler : AudioSampler
    {
        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            if (format.Channels == 1)
            {
                return Task.FromResult(audio);
            }

            return Task.FromResult(audio.WithData(
                data: audio
                    .GroupByLengthAsync(format.Channels, cancellationToken)
                    .SelectAsync(x => x.Average(), cancellationToken),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, 1, format.Type)));
        }
    }
}
