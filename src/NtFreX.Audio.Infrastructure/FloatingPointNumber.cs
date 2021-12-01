using System;

namespace NtFreX.Audio.Infrastructure
{
    /// <summary>
    /// The idea of this class is to provide a double as a value big enough to hold float, double or an strange 16 floating point number
    /// </summary>
    public static class FloatingPointNumber
    {
        public static double FromGivenBits(Memory<byte> value, bool isLittleEndian = true)
        {
            return value.Length switch
            {
                2 => value.ToInt16(isLittleEndian) / (short.MaxValue + 1f),
                4 => value.ToFloat(isLittleEndian),
                8 => value.ToDouble(isLittleEndian),
                _ => throw new ArgumentException($"Floating point numbers with {value.Length * 8} bits are not supported.", nameof(value)),
            };
        }

        public static Memory<byte> ToRequiredBits(uint bits, double value = 0, bool isLittleEndian = true)
        {
            // 16 bits is no supported according to https://de.wikipedia.org/wiki/Gleitkommazahlen_in_digitaler_Audioanwendung
            // https://markheath.net/post/convert-16-bit-pcm-to-ieee-float
            return bits switch
            {
                16 => ((short)(value * (short.MaxValue + 1f))).ToMemory(isLittleEndian),
                32 => ((float)value).ToMemory(isLittleEndian),
                64 => value.ToMemory(isLittleEndian),
                _ => throw new ArgumentException($"Floating point numbers with {bits} bits are not supported.", nameof(bits)),
            };
        }
    }
}
