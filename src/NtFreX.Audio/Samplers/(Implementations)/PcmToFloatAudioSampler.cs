using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
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
            var samples = audio
                .GetAudioSamplesAsync(cancellationToken)
                .SelectAsync(x => new Sample(value: x.Value / max, definition: new SampleDefinition(AudioFormatType.IeeFloat, x.Definition.Bits, x.Definition.IsLittleEndian)), cancellationToken);

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithAudioFormat(AudioFormatType.IeeFloat))
                .WithDataSubChunk(x => x.WithData(samples)));
        }
    }
}
