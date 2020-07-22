using System.Linq;

namespace NtFreX.Audio.Infrastructure
{
    public static class ChannelFactory
    {
        // TODO: validate default mapping
        private static readonly ChannelConfig[] ChannelConfigs = new ChannelConfig[]
        {
            new ChannelConfig(Speaker.Mono, 1,        true),
            new ChannelConfig(Speaker.OnePointOne, 2, false),
            new ChannelConfig(Speaker.Stereo, 2,      true),
            new ChannelConfig(Speaker.TwoPointOne, 3, true),
            new ChannelConfig(Speaker.ThreePointZero, 3, false),
            new ChannelConfig(Speaker.ThreePointOne, 4, true),
            new ChannelConfig(Speaker.Quad, 4,        false),
            new ChannelConfig(Speaker.Surround, 4,    false),
            new ChannelConfig(Speaker.FivePointZero, 5, true),
            new ChannelConfig(Speaker.FivePointOne, 6, true),
            new ChannelConfig(Speaker.SevenPointZero, 7, true),
            new ChannelConfig(Speaker.SevenPointOne, 8, true),
            new ChannelConfig(Speaker.FivePointOneSurround, 6, false),
            new ChannelConfig(Speaker.SevenPointOneSurround, 8, false)
        };

        public static Speaker GetDefaultMapping(ushort channelCount)
            => ChannelConfigs.First(x => x.Channels == channelCount && x.IsDefault).Speaker;

        public static ushort GetChannels(Speaker speaker)
            => ChannelConfigs.First(x => x.Speaker == speaker).Channels;

        internal class ChannelConfig
        {
            public Speaker Speaker { get; }
            public ushort Channels { get; }
            public bool IsDefault { get; }

            public ChannelConfig(Speaker speaker, ushort channels, bool isDefault)
            {
                this.Speaker = speaker;
                this.Channels = channels;
                this.IsDefault = isDefault;
            }
        }
    }
}