using System;

namespace NtFreX.Audio.Extensions
{
    internal static class BitConverterExtensions
    {
        public static byte[] ToByteArray(this int value)
            => BitConverter.GetBytes(value);

        public static byte[] ToByteArray(this uint value)
            => BitConverter.GetBytes(value);

        public static byte[] ToByteArray(this ushort value)
            => BitConverter.GetBytes(value);
    }
}
