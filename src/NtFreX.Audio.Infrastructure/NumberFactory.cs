using System;

namespace NtFreX.Audio.Infrastructure
{
    public static class NumberFactory
    {
        public static byte[] DeconstructNumber(SampleDefinition definition, double value)
        {
            return definition.Type == AudioFormatType.Pcm ? Number.ToRequiredBits(definition.Bits, (long)value, definition.IsLittleEndian) :
                   definition.Type == AudioFormatType.IeeFloat ? FloatingPointNumber.ToRequiredBits(definition.Bits, value, definition.IsLittleEndian) :
                   throw new NotImplementedException("The given audio format type is not supported");
        }

        public static double ConstructNumber(AudioFormatType type, bool isLittleEndian, byte[] value)
        {
            return type == AudioFormatType.Pcm ? Number.FromGivenBits(value, isLittleEndian) :
                   type == AudioFormatType.IeeFloat ? FloatingPointNumber.FromGivenBits(value, isLittleEndian) :
                         throw new NotImplementedException("The given audio format type is not supported");
        }
    }
}
