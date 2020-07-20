using System;

namespace NtFreX.Audio.Wasapi.Interop
{
    internal static class MediaType
    {
        // Ksmedia.h 
        public static Guid STATIC_KSDATAFORMAT_SUBTYPE_PCM => new Guid("00000001-0000-0010-8000-00aa00389b71");
        public static Guid STATIC_KSDATAFORMAT_SUBTYPE_IEEE_FLOAT => new Guid("00000003-0000-0010-8000-00aa00389b71");
        public static Guid STATIC_KSDATAFORMAT_SUBTYPE_WAVEFORMATEX => new Guid("00000000-0000-0010-8000-00aa00389b71");
    }
}
