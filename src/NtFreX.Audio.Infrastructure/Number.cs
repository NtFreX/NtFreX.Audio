using System;

namespace NtFreX.Audio.Infrastructure
{
    /// <summary>
    /// The idea of this class is to provide a long as a value big enough to hold sbyte, int16, int32 or int64
    /// </summary>
    public static class Number
    {
        public static long FromGivenBits(Memory<byte> value, bool isLittleEndian = true)
        {
            return value.Length switch
            {
                1 => (sbyte)value.Span[0],
                2 => value.ToInt16(isLittleEndian),
                4 => value.ToInt32(isLittleEndian),
                8 => value.ToInt64(isLittleEndian),
                _ => throw new ArgumentException($"Numbers with {value.Length * 8} bits are not supported.", nameof(value)),
            };
        }

        public static Memory<byte> ToRequiredBits(uint bits, long value = 0, bool isLittleEndian = true)
        {
            return bits switch
            {
                8 => new[] { (byte)value },
                16 => ((short)value).ToByteArray(isLittleEndian),
                32 => ((int)value).ToByteArray(isLittleEndian),
                64 => value.ToByteArray(isLittleEndian),
                _ => throw new ArgumentException($"Numbers with {bits} bits are not supported.", nameof(bits)),
            };
        }
    }
}
