namespace NtFreX.Audio.Infrastructure
{
    public struct Sample
    {
        private readonly byte[] value;

        public AudioFormatType Type { get; }

        public ushort Bits { get; }

        public Sample(byte[] value, ushort bits, AudioFormatType type)
        {
            Bits = bits;
            Type = type;

            this.value = value;
        }

        public byte[] AsByteArray() => value;
        public long AsInt64() => value.ToInt64();

        public override string ToString() => AsInt64().ToString();

        public static Sample FromValue(long value, ushort bits, AudioFormatType type) => new Sample(value.ToByteArray(bits / 8), bits, type);
        public static Sample Zero(ushort bits, AudioFormatType type) => new Sample(new byte[bits / 8], bits, type);

        //TODO: fix overflow for 64bit audio => limit?
        //TODO: validate bitness? validate type?
        //TODO: conversion improvement!
        public static Sample operator +(Sample a, Sample b)
            => new Sample((a.AsInt64() + b.AsInt64()).ToByteArray(a.Bits / 8), a.Bits, a.Type);
        public static Sample operator -(Sample a, Sample b)
            => new Sample((a.AsInt64() - b.AsInt64()).ToByteArray(a.Bits / 8), a.Bits, a.Type);
        public static Sample operator +(Sample a, double b)
            => new Sample(((long) (a.AsInt64() + b)).ToByteArray(a.Bits / 8), a.Bits, a.Type);
        public static Sample operator -(Sample a, double b)
            => new Sample(((long) (a.AsInt64() - b)).ToByteArray(a.Bits / 8), a.Bits, a.Type);
        public static Sample operator /(Sample a, double b)
            => new Sample(((long)(a.AsInt64() / b)).ToByteArray(a.Bits / 8), a.Bits, a.Type);
        public static Sample operator *(Sample a, double b)
            => new Sample(((long)(a.AsInt64() * b)).ToByteArray(a.Bits / 8), a.Bits, a.Type);
        public static bool operator <(Sample a, Sample b)
            => a.AsInt64() < b.AsInt64();
        public static bool operator >(Sample a, Sample b)
            => a.AsInt64() > b.AsInt64();
    }
}
