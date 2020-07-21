using System.Collections.Generic;

namespace NtFreX.Audio.Wasapi
{
    internal static class ChannelFactory
    {
        // TODO: validate default mapping
        private static readonly Dictionary<ushort, Interop.Speaker> DefaultChannelMapping = new Dictionary<ushort, Interop.Speaker>()
        {
            { 1, Interop.Speaker.KSAUDIO_MONO },
            { 2, Interop.Speaker.KSAUDIO_STEREO },
            { 3, Interop.Speaker.KSAUDIO_2POINT1 },
            { 4, Interop.Speaker.KSAUDIO_3POINT1 },
            { 5, Interop.Speaker.KSAUDIO_5POINT0 },
            { 6, Interop.Speaker.KSAUDIO_5POINT1 },
            { 7, Interop.Speaker.KSAUDIO_7POINT0 },
            { 8, Interop.Speaker.KSAUDIO_7POINT1 }
        };

        public static Interop.Speaker GetDefaultMapping(ushort channelCount)
            => DefaultChannelMapping[channelCount];

        //public static ushort GetChannels(Interop.Speaker speaker)
        //{
        //    foreach(var pair in DefaultChannelMapping)
        //    {
        //        if(pair.Value == speaker)
        //        {
        //            return pair.Key;
        //        }
        //    }
        //    throw new Exception("The given speaker mapping is not known");
        //}
    }
}
