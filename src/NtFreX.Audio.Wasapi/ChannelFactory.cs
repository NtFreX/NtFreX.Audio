using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Wasapi
{
    internal static class ChannelFactory
    {
        private static readonly Dictionary<ushort, Interop.Speaker> DefaultChannelMapping = new Dictionary<ushort, Interop.Speaker>()
        {
            // { 1,  } //TODO
            { 2, Interop.Speaker.FRONT_LEFT | Interop.Speaker.FRONT_RIGHT }
        };

        public static Interop.Speaker GetDefaultMapping(ushort channelCount)
            => DefaultChannelMapping[channelCount];

        public static ushort GetChannels(Interop.Speaker speaker)
        {
            foreach(var pair in DefaultChannelMapping)
            {
                if(pair.Value == speaker)
                {
                    return pair.Key;
                }
            }
            throw new Exception("The given speaker mapping is not known");
        }
    }
}
