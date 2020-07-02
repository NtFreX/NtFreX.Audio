using System;

namespace NtFreX.Audio.Extensions
{
    internal static class ByteArrayExtensions
    {
        public static uint TakeUInt(this byte[] value, int start)
        {
            return BitConverter.ToUInt32(value, start);
        }

        public static int TakeInt(this byte[] value, int start)
        {
            return BitConverter.ToInt32(value, start);
        }

        public static ushort TakeUShort(this byte[] value, int start)
        {
            return BitConverter.ToUInt16(value, start);
        }
    }
}
