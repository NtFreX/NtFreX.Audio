using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class ChannelAudioSampler : AudioSampler
    {
        private readonly Speakers targetSpeaker;
        private readonly SampleChannelMapping[] channelMappings = new SampleChannelMapping[]
        {
            new MonoSampleChannelMapping(),
            new OnePointOneSampleChannelMapping(),
            new StereoSampleChannelMapping(),
            new TwoPointOneSampleChannelMapping(),
            new ThreePointZeroSampleChannelMapping(),
            new ThreePointOneSampleChannelMapping(),
            new QuadSampleChannelMapping(),
            new SurroundSampleChannelMapping(),
            new FivePointZeroSampleChannelMapping(),
            new FivePointOneSampleChannelMapping(),
            new SevenPointZeroSampleChannelMapping(),
            new SevenPointOneSampleChannelMapping(),
            new FivePointOneSurroundSampleChannelMapping(),
            new SevenPointOneSurroundSampleChannelMapping()
        };
        private readonly Dictionary<Speakers, Func<SampleChannelMapping, Func<Sample[], Sample[]>>> converterResolver = new Dictionary<Speakers, Func<SampleChannelMapping, Func<Sample[], Sample[]>>>()
        {
            { Speakers.Mono, x => x.ToMono },
            { Speakers.OnePointOne, x => x.ToOnePointOne },
            { Speakers.Stereo, x => x.ToStereo },
            { Speakers.TwoPointOne, x => x.ToTwoPointOne },
            { Speakers.ThreePointZero, x => x.ToThreePointZero },
            { Speakers.ThreePointOne, x => x.ToThreePointOne },
            { Speakers.Quad, x => x.ToQuad },
            { Speakers.Surround, x => x.ToSurround },
            { Speakers.FivePointZero, x => x.ToFivePointZero },
            { Speakers.FivePointOne, x => x.ToFivePointOne },
            { Speakers.SevenPointZero, x => x.ToSevenPointZero },
            { Speakers.SevenPointOne, x => x.ToSevenPointOne },
            { Speakers.FivePointOneSurround, x => x.ToFivePointOneSurround },
            { Speakers.SevenPointOneSurround, x => x.ToSevenPointOneSurround }
        };

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
            var targetChannels = ChannelFactory.GetChannels(targetSpeaker);
            var format = audio.GetFormat();
            var factor = targetChannels / (double) format.Channels;

            Speakers sourceSpeaker = ChannelFactory.GetDefaultMapping(format.Channels);
            var channelMapping = channelMappings.First(x => x.Speaker == sourceSpeaker);
            var converter = converterResolver[targetSpeaker].Invoke(channelMapping);

            return Task.FromResult(audio.WithData(
                data: audio
                    .GroupByLengthAsync(format.Channels, cancellationToken)
                    .SelectAsync(x => converter.Invoke(x), cancellationToken)
                    .SelectManyAsync(x => x, (long)(factor * audio.GetDataLength()), cancellationToken),
                format: new AudioFormat(format.SampleRate, format.BitsPerSample, targetChannels, format.Type)));
        }
    }
}
