using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Wasapi
{
    internal static class MediaTypeFactory
    {
        // https://de.wikipedia.org/wiki/RIFF_WAVE
        private static readonly Dictionary<AudioFormatType, Guid> MediaTypes = new Dictionary<AudioFormatType, Guid>()
        {
            { AudioFormatType.PCM, Interop.MediaType.STATIC_KSDATAFORMAT_SUBTYPE_PCM },
            { AudioFormatType.IEE_FLOAT, Interop.MediaType.STATIC_KSDATAFORMAT_SUBTYPE_IEEE_FLOAT }
        };

        public static Guid GetMediaType(AudioFormatType mediaType)
            => MediaTypes[mediaType];

        public static AudioFormatType GetMediaType(Guid mediaType)
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
