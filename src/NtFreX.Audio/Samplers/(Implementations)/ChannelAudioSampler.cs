using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class ChannelAudioSampler : AudioSampler
    {
        private readonly Speakers targetSpeaker;

        public ChannelAudioSampler(Speakers targetSpeaker)
        {
            //TODO: get wasapi channel mapping
            this.targetSpeaker = targetSpeaker;
        }

        public ChannelAudioSampler(ushort targetChannels)
        {
            this.targetSpeaker = ChannelFactory.GetDefaultMapping(targetChannels);
        }

        public override Task<IntermediateEnumerableAudioContainer> SampleAsync(IntermediateEnumerableAudioContainer audio, CancellationToken cancellationToken = default)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            // TODO: check if channel config is allready matching
            var format = audio.GetFormat();
            var targetChannels = ChannelFactory.GetChannels(targetSpeaker); 
            var sourceSpeaker = ChannelFactory.GetDefaultMapping(format.Channels);
            var channelMapping = ChannelMappingFactory.Instance.GetChannelMapping(sourceSpeaker);
            var converter = ChannelMappingFactory.Instance.GetSampleConverter(targetSpeaker, channelMapping);
            var factor = targetChannels / (double) format.Channels;

            return Task.FromResult(audio.WithData(
                data: audio
                    .GroupByLengthAsync(format.Channels, cancellationToken)
                    .SelectAsync(x => converter.Invoke(x), cancellationToken)
                    .SelectManyAsync(x => x, (long)(factor * audio.GetDataLength()), cancellationToken),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, targetChannels, format.Type)));
        }
    }
}
