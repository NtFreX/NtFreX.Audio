using System.Globalization;

namespace NtFreX.Audio.Infrastructure
{
    public struct Sample
    {
        public AudioFormatType Type { get; }

        //TODO: remove from here?
        public ushort Bits { get; }
        //TODO: support float
        public long Value { get; }

        private byte[] cache;

        public Sample(byte[] value, ushort bits, AudioFormatType type)
        {
            Bits = bits;
            Type = type;

            this.cache = value;
            this.Value = value.ToInt64();
        }

        public Sample(long value, ushort bits, AudioFormatType type)
        {
            Bits = bits;
            Type = type;

            this.cache = null;
            this.Value = value;
        }

        public byte[] AsByteArray()
        {
            if(cache == null)
            {
                cache = Value.ToByteArray(Bits / 8);
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

        private static long LimitTo(ushort bits, long value)
        {
            var max = (long)System.Math.Pow(256, bits / 8) / 2;
            var min = max * -1;
            return value < 0 ? System.Math.Max(min, value) : System.Math.Min(max, value);
        }
    }
}
