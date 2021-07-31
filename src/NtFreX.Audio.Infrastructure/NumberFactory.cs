using System;
using System.Collections.Generic;

namespace NtFreX.Audio.Infrastructure
{
    public static class NumberFactory
    {
        private static Dictionary<string, Memory<byte>> deconstructCache = new Dictionary<string, Memory<byte>>();
        private static Dictionary<string, double> constructCache = new Dictionary<string, double>();

        private static string ToDeconstructKey(SampleDefinition definition, double value)
            => $"{definition.Type}:{definition.Bytes}:{definition.IsLittleEndian}:{value}";

        //private static string ToConstructKey(AudioFormatType type, bool isLittleEndian, Memory<byte> value)
        //    => $"{type}:{isLittleEndian}:{}";

        public static Memory<byte> DeconstructNumber(SampleDefinition definition, double value)
        {
            var key = ToDeconstructKey(definition, value);
            if (deconstructCache.TryGetValue(key, out var result))
            {
                return result;
            }

            result = definition.Type == AudioFormatType.Pcm ? Number.ToRequiredBits(definition.Bits, (long)value, definition.IsLittleEndian) :
                     definition.Type == AudioFormatType.IeeFloat ? FloatingPointNumber.ToRequiredBits(definition.Bits, value, definition.IsLittleEndian) :
                     throw new NotImplementedException("The given audio format type is not supported");

            deconstructCache.Add(key, result);
            return result;
        }

        public static double ConstructNumber(AudioFormatType type, bool isLittleEndian, Memory<byte> value)
        {
            //var key = ToConstructKey(type, isLittleEndian, value);
            //if (constructCache.TryGetValue(key, out var result))
            //{
            //    return result;
            //}

            var result = type == AudioFormatType.Pcm ? Number.FromGivenBits(value, isLittleEndian) :
                     type == AudioFormatType.IeeFloat ? FloatingPointNumber.FromGivenBits(value, isLittleEndian) :
                     throw new NotImplementedException("The given audio format type is not supported");

            //constructCache.Add(key, result);
            return result;
        }
    }
}
