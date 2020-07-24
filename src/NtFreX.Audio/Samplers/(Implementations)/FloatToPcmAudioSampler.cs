using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class FloatToPcmAudioSampler : AudioSampler
    {
        [return: NotNull]
        public override Task<WaveEnumerableAudioContainer> SampleAsync([NotNull] WaveEnumerableAudioContainer audio, [MaybeNull] CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            if (audio.Format.Type == AudioFormatType.Pcm)
            {
                return Task.FromResult(audio);
            }
            if (audio.Format.Type != AudioFormatType.IeeFloat)
            {
                throw new ArgumentException("The given format must be float", nameof(audio));
            }

            var max = (System.Math.Pow(2, audio.Format.BitsPerSample) / 2) - 1;
            var isLittleEndian = audio.IsDataLittleEndian();
            var samples = audio.GetAudioSamplesAsync().SelectAsync(x => new Sample((x.Value - 0.5) * 2 * max, x.Bits, AudioFormatType.Pcm, isLittleEndian));

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithAudioFormat(AudioFormatType.Pcm))
                .WithDataSubChunk(x => x.WithData(samples)));
        }
    }
}
