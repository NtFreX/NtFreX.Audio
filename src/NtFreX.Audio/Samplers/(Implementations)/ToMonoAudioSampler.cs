using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
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
                data: InterleaveChannelData(audio, format, cancellationToken)
                    .ToNonSeekable(audio.GetDataLength() / format.Channels),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, 1, format.Type)));
        }

        private static async IAsyncEnumerable<Sample> InterleaveChannelData(ISeekableAsyncEnumerable<Sample> audio, IAudioFormat format, CancellationToken cancellationToken)
        {
            var temp = new Sample[format.Channels];
            var counter = 0;
            await foreach(var sample in audio)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                temp[counter++] = sample;
                if (counter == format.Channels)
                {
                    yield return temp.Average();
                    counter = 0;
                }
            }
        }
    }
}
