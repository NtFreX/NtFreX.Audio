using System;

namespace NtFreX.Audio.Infrastructure
{
    public static class NumberFactory
    {
        public static Memory<byte> DeconstructNumber(SampleDefinition definition, double value)
        {
            return definition.Type == AudioFormatType.Pcm ? Number.ToRequiredBits(definition.Bits, (long)value, definition.IsLittleEndian) :
                   definition.Type == AudioFormatType.IeeFloat ? FloatingPointNumber.ToRequiredBits(definition.Bits, value, definition.IsLittleEndian) :
                   throw new NotImplementedException("The given audio format type is not supported");
        }

        public static void DeconstructNumber(SampleDefinition definition, double value, Span<byte> buffer)
        {
            if (definition.Type == AudioFormatType.Pcm)
            {
                Number.ToRequiredBits(definition.Bits, buffer, (long)value, definition.IsLittleEndian);
            }
            else if (definition.Type == AudioFormatType.IeeFloat)
            {
                FloatingPointNumber.ToRequiredBits(definition.Bits, buffer, value, definition.IsLittleEndian);
            }
            else
            {
                throw new NotImplementedException("The given audio format type is not supported");
            }
        }

        public static double ConstructNumber(AudioFormatType type, bool isLittleEndian, Memory<byte> value)
        {
            return type == AudioFormatType.Pcm ? Number.FromGivenBits(value, isLittleEndian) :
                   type == AudioFormatType.IeeFloat ? FloatingPointNumber.FromGivenBits(value, isLittleEndian) :
                         throw new NotImplementedException("The given audio format type is not supported");
        }
    }
}
