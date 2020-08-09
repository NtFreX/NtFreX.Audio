using System;

namespace NtFreX.Audio.Infrastructure
{
    /// <summary>
    /// The idea of this class is to provide a long as a value big enough to hold sbyte, int16, int32 or int64
    /// </summary>
    public static class Number
    {
        public static long FromGivenBits(byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return value.Length switch
            {
                1 => (sbyte)value[0],
                2 => value.ToInt16(isLittleEndian),
                4 => value.ToInt32(isLittleEndian),
                8 => value.ToInt64(isLittleEndian),
                _ => throw new ArgumentException($"Numbers with {value.Length * 8} bits are not supported.", nameof(value)),
            };
        }

        public static byte[] ToRequiredBits(uint bits, long value = 0, bool isLittleEndian = true)
        {
            return (bits / 8) switch
            {
                1 => new[] { (byte)value },
                2 => ((short)value).ToByteArray(isLittleEndian),
                4 => ((int)value).ToByteArray(isLittleEndian),
                8 => value.ToByteArray(isLittleEndian),
                _ => throw new ArgumentException($"Numbers with {bits} bits are not supported.", nameof(bits)),
            };
        }
    }
}
