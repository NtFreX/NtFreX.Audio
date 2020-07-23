using System.Linq;

namespace NtFreX.Audio.Infrastructure
{
    public static class ChannelFactory
    {
        // TODO: validate default mapping
        private static readonly ChannelConfig[] ChannelConfigs = new ChannelConfig[]
        {
            new ChannelConfig(Speakers.Mono, 1,        true),
            new ChannelConfig(Speakers.OnePointOne, 2, false),
            new ChannelConfig(Speakers.Stereo, 2,      true),
            new ChannelConfig(Speakers.TwoPointOne, 3, true),
            new ChannelConfig(Speakers.ThreePointZero, 3, false),
            new ChannelConfig(Speakers.ThreePointOne, 4, true),
            new ChannelConfig(Speakers.Quad, 4,        false),
            new ChannelConfig(Speakers.Surround, 4,    false),
            new ChannelConfig(Speakers.FivePointZero, 5, true),
            new ChannelConfig(Speakers.FivePointOne, 6, true),
            new ChannelConfig(Speakers.SevenPointZero, 7, true),
            new ChannelConfig(Speakers.SevenPointOne, 8, true),
            new ChannelConfig(Speakers.FivePointOneSurround, 6, false),
            new ChannelConfig(Speakers.SevenPointOneSurround, 8, false)
        };

        public static Speakers GetDefaultMapping(ushort channelCount)
            => ChannelConfigs.First(x => x.Channels == channelCount && x.IsDefault).Speaker;

        public static ushort GetChannels(Speakers speaker)
            => ChannelConfigs.First(x => x.Speaker == speaker).Channels;

        internal class ChannelConfig
        {
            public Speakers Speaker { get; }
            public ushort Channels { get; }
            public bool IsDefault { get; }

            public ChannelConfig(Speakers speaker, ushort channels, bool isDefault)
            {
                this.Speaker = speaker;
                this.Channels = channels;
                this.IsDefault = isDefault;
            }
        }
    }
}