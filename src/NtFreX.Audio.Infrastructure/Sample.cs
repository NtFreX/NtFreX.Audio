using System;
using System.Globalization;

namespace NtFreX.Audio.Infrastructure
{
    public struct Sample
    {
        public AudioFormatType Type { get; }

        //TODO: remove from here?
        public ushort Bits { get; }
        public double Value { get; }
        public bool IsLittleEndian { get; }

        private byte[] cache;

        public Sample(byte[] value, ushort bits, AudioFormatType type, bool isLittleEndian = true)
        {
            Bits = bits;
            Type = type;
            IsLittleEndian = isLittleEndian;

            this.cache = value;
            this.Value = type == AudioFormatType.PCM ? value.ToInt64(IsLittleEndian) : 
                         type == AudioFormatType.IEE_FLOAT && bits == 64 ? value.ToDouble(IsLittleEndian) :
                         type == AudioFormatType.IEE_FLOAT && bits == 32 ? value.ToFloat(IsLittleEndian) :
                         type == AudioFormatType.IEE_FLOAT && bits == 16 ? value.ToInt16(IsLittleEndian) / 32768f :
                         throw new NotImplementedException();
        }

        public Sample(double value, ushort bits, AudioFormatType type, bool isLittleEndian = true)
        {
            Bits = bits;
            Type = type;
            IsLittleEndian = isLittleEndian;

            this.cache = null;
            this.Value = value;
        }

        public byte[] AsByteArray()
        {
            if(cache == null)
            {
                if (Type == AudioFormatType.PCM)
                {
                    cache = ((long)Value).ToByteArray(Bits / 8, IsLittleEndian);
                }
                else if (Type == AudioFormatType.IEE_FLOAT && Bits == 16)
                {
                    // https://markheath.net/post/convert-16-bit-pcm-to-ieee-float
                    cache = ((short)(Value * 32768f)).ToByteArray(IsLittleEndian);
                }
                else if (Type == AudioFormatType.IEE_FLOAT && Bits == 32)
                {
                    cache = ((float)Value).ToByteArray(IsLittleEndian);
                }
                else if (Type == AudioFormatType.IEE_FLOAT && Bits == 64)
                {
                    cache = Value.ToByteArray(IsLittleEndian);
                }
                else
                {
                    throw new NotImplementedException("The given audio format type is not supported");
                }
            }
            return cache;
        }

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

        public static Sample Zero(ushort bits, AudioFormatType type) => new Sample(0, bits, type);

        //TODO: fix overflow for 64bit audio => limit?
        //TODO: validate bitness? validate type?
        //TODO: conversion improvement!
        public static Sample operator +(Sample a, Sample b)
            => new Sample(LimitTo(a.Bits, a.Value + b.Value), a.Bits, a.Type);
        public static Sample operator -(Sample a, Sample b)
            => new Sample(LimitTo(a.Bits, a.Value - b.Value), a.Bits, a.Type);
        public static Sample operator +(Sample a, double b)
            => new Sample(LimitTo(a.Bits, (long) (a.Value + b)), a.Bits, a.Type);
        public static Sample operator -(Sample a, double b)
            => new Sample(LimitTo(a.Bits, (long) (a.Value - b)), a.Bits, a.Type);
        public static Sample operator /(Sample a, double b)
            => new Sample(LimitTo(a.Bits, (long)(a.Value / b)), a.Bits, a.Type);
        public static Sample operator *(Sample a, double b)
            => new Sample(LimitTo(a.Bits, (long)(a.Value * b)), a.Bits, a.Type);
        public static bool operator <(Sample a, Sample b)
            => a.Value < b.Value;
        public static bool operator >(Sample a, Sample b)
            => a.Value > b.Value;

        private static double LimitTo(ushort bits, double value)
        {
            var max = Math.Pow(256, bits / 8) / 2;
            var min = max * -1;
            return value < 0 ? Math.Max(min, value) : Math.Min(max, value);
        }
    }
}
