using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class PcmToFloatAudioSampler : AudioSampler
    {
        [return: NotNull]
        public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));
            
            if (audio.Format.Type == AudioFormatType.IeeFloat)
            {
                return Task.FromResult(audio);
            }
            if (audio.Format.Type != AudioFormatType.Pcm)
            {
                throw new ArgumentException("The given format must be pcm", nameof(audio));
            }

            var max = (System.Math.Pow(2, audio.Format.BitsPerSample) / 2) - 1;
            var samples = audio.GetAudioSamplesAsync().SelectAsync(x => new Sample((x.Value / max / 2D) + 0.5, x.Bits, AudioFormatType.IeeFloat));

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithAudioFormat(AudioFormatType.IeeFloat))
                .WithDataSubChunk(x => x.WithData(samples)));
        }
    }
}
