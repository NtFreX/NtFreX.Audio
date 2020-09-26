using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class FromMonoAudioSampler : AudioSampler
    {
        private readonly int targetChannels;

        public FromMonoAudioSampler(int targetChannels)
        {
            this.targetChannels = targetChannels;
        }

        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var format = audio.GetFormat();
            if(format.Channels != 1)
            {
                throw new ArgumentException("Only mono is supported");
            }

            if (format.Channels == targetChannels)
            {
                return Task.FromResult(audio);
            }

            return Task.FromResult(audio.WithData(
                data: MultiplicateChannelData(audio, cancellationToken)
                    .ToNonSeekable(audio.GetDataLength() * targetChannels),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, (ushort) targetChannels, format.Type)));
        }

        private async IAsyncEnumerable<Sample> MultiplicateChannelData(ISeekableAsyncEnumerable<Sample> audio, CancellationToken cancellationToken)
        {
            await foreach(var value in audio)
            {
                if(cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                for (var i = 0; i < targetChannels; i++)
                {
                    yield return value;
                }
            }
        }
    }
}
