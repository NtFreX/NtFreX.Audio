using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NtFreX.Audio.Samplers
{
    internal class ChannelMappingFactory
    {
        public static ChannelMappingFactory Instance { get; } = new ChannelMappingFactory();
        private ChannelMappingFactory() { }

        //valid source
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
        // valid target
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

        public SampleChannelMapping GetChannelMapping(Speakers speaker)
            => channelMappings.First(x => x.Speaker == speaker);

        public Func<Sample[], Sample[]> GetSampleConverter(Speakers speaker, SampleChannelMapping channelMapping)
            => converterResolver[speaker].Invoke(channelMapping);

        public bool IsValidTargetSpeaker(ushort targetSpeaker)
            => IsValidTargetSpeaker(ChannelFactory.GetDefaultMapping(targetSpeaker));
        public bool IsValidTargetSpeaker(Speakers targetSpeaker)
            => converterResolver.ContainsKey(targetSpeaker);

        public bool IsValidSourceSpeaker(ushort sourceSpeaker)
            => IsValidSourceSpeaker(ChannelFactory.GetDefaultMapping(sourceSpeaker));
        public bool IsValidSourceSpeaker(Speakers sourceSpeaker)
            => channelMappings.Any(x => x.Speaker == sourceSpeaker);

        public bool CanMap(Speakers source, Speakers target)
            => IsValidSourceSpeaker(source) && IsValidTargetSpeaker(target);
        public bool CanMap(ushort source, ushort target)
            => IsValidSourceSpeaker(source) && IsValidTargetSpeaker(target);
    }
}
