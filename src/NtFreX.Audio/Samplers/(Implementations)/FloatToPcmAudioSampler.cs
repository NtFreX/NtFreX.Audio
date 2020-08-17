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

            return Task.FromResult(audio
                .WithFmtSubChunk(x => x.WithAudioFormat(AudioFormatType.Pcm))
                .WithDataSubChunk(x => x.WithData(SampleInnerAsync(audio, cancellationToken))));
        }

        private static async IAsyncEnumerable<Sample> SampleInnerAsync(WaveEnumerableAudioContainer audio, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var max = (System.Math.Pow(2, audio.Format.BitsPerSample) / 2) - 1;
            var defintition = new SampleDefinition(AudioFormatType.Pcm, audio.Format.BitsPerSample, audio.IsDataLittleEndian());
            await foreach(var sample in audio.GetAudioSamplesAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return new Sample(sample.Value * max, defintition);
            }
        }
    }
}
