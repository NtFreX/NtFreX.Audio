using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithAudioFormat(AudioFormatType.IeeFloat))
                .WithDataSubChunk(x => x.WithData(SampleInnerAsync(audio, cancellationToken))));
        }

        private static async IAsyncEnumerable<Sample> SampleInnerAsync(WaveEnumerableAudioContainer audio, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var max = (System.Math.Pow(2, audio.Format.BitsPerSample) / 2) - 1;
            var definition = new SampleDefinition(AudioFormatType.IeeFloat, audio.Format.BitsPerSample, audio.IsDataLittleEndian());
            await foreach (var sample in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return new Sample(sample.Value / max, definition);
            }
        }
    }
}
