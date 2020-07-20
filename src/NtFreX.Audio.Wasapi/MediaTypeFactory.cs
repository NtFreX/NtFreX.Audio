using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Wasapi
{
    internal static class MediaTypeFactory
    {
        // https://de.wikipedia.org/wiki/RIFF_WAVE
        private static readonly Dictionary<int, Guid> MediaTypes = new Dictionary<int, Guid>()
        {
            { 0x0001, Interop.MediaType.STATIC_KSDATAFORMAT_SUBTYPE_PCM },
            { 0x0003, Interop.MediaType.STATIC_KSDATAFORMAT_SUBTYPE_IEEE_FLOAT }
        };

        public static Guid GetMediaType(int mediaType)
            => MediaTypes[mediaType];

        public static int GetMediaType(Guid mediaType)
        {
            foreach(var pair in MediaTypes)
            {
                if(pair.Value == mediaType)
                {
                    return pair.Key;
                }
            }
            throw new Exception("The given media type is not known");
        }
    }
}
