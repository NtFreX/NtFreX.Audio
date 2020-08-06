using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace NtFreX.Audio.Infrastructure
{
    public struct Sample : IEquatable<Sample>
    {
        public double Value { get; }
        public SampleDefinition Definition { get; }

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly
        private byte[]? cache;
#pragma warning restore SA1011 // Closing square brackets should be spaced correctly

        public Sample(byte[] value, SampleDefinition definition)
        {
            Definition = definition;

            this.cache = value;
            this.Value = definition.Type == AudioFormatType.Pcm ? value.ToInt64(definition.IsLittleEndian) :
                         definition.Type == AudioFormatType.IeeFloat && definition.Bits == 64 ? value.ToDouble(definition.IsLittleEndian) :
                         definition.Type == AudioFormatType.IeeFloat && definition.Bits == 32 ? value.ToFloat(definition.IsLittleEndian) :
                         definition.Type == AudioFormatType.IeeFloat && definition.Bits == 16 ? value.ToInt16(definition.IsLittleEndian) / 32768f :
                         throw new NotImplementedException();
        }

        public Sample(double value, SampleDefinition definition)
        {
            Definition = definition;

            this.cache = null;
            this.Value = value;
        }

        public static Sample Zero(SampleDefinition definition) => new Sample(0, definition);

        //TODO: is limit a good idea?
        public static Sample operator +(Sample a, Sample b)
            => a.Definition == b.Definition ? new Sample(LimitTo(a.Definition.Bits, a.Value + b.Value), a.Definition) : throw new Exception();
        public static Sample operator -(Sample a, Sample b)
            => a.Definition == b.Definition ? new Sample(LimitTo(a.Definition.Bits, a.Value - b.Value), a.Definition) : throw new Exception();
        public static Sample operator +(Sample a, double b)
            => new Sample(LimitTo(a.Definition.Bits, (long) (a.Value + b)), a.Definition);
        public static Sample operator -(Sample a, double b)
            => new Sample(LimitTo(a.Definition.Bits, (long) (a.Value - b)), a.Definition);
        public static Sample operator /(Sample a, double b)
            => new Sample(LimitTo(a.Definition.Bits, (long)(a.Value / b)), a.Definition);
        public static Sample operator *(Sample a, double b)
            => new Sample(LimitTo(a.Definition.Bits, (long)(a.Value * b)), a.Definition);
        public static bool operator <(Sample a, Sample b) => a.Definition == b.Definition ? a.Value < b.Value : throw new Exception();
        public static bool operator >(Sample a, Sample b) => a.Definition == b.Definition ? a.Value > b.Value : throw new Exception();
        public static bool operator ==(Sample left, Sample right) => left.Equals(right);
        public static bool operator !=(Sample left, Sample right) => !(left == right);

        public static Sample Add(Sample left, Sample right) => left + right;
        public static Sample Subtract(Sample left, Sample right) => left - right;
        public static Sample Divide(Sample left, double right) => left / right;
        public static Sample Multiply(Sample left, double right) => left * right;

        public byte[] AsByteArray()
        {
            if (cache == null)
            {
                if (Definition.Type == AudioFormatType.Pcm)
                {
                    cache = ((long)Value).ToByteArray(Definition.Bits / 8, Definition.IsLittleEndian);
                }
                else if (Definition.Type == AudioFormatType.IeeFloat && Definition.Bits == 16)
                {
                    // This is no supported according to https://de.wikipedia.org/wiki/Gleitkommazahlen_in_digitaler_Audioanwendung
                    // https://markheath.net/post/convert-16-bit-pcm-to-ieee-float
                    cache = ((short)(Value * 32768f)).ToByteArray(Definition.IsLittleEndian);
                }
                else if (Definition.Type == AudioFormatType.IeeFloat && Definition.Bits == 32)
                {
                    cache = ((float)Value).ToByteArray(Definition.IsLittleEndian);
                }
                else if (Definition.Type == AudioFormatType.IeeFloat && Definition.Bits == 64)
                {
                    cache = Value.ToByteArray(Definition.IsLittleEndian);
                }
                else
                {
                    throw new NotImplementedException("The given audio format type is not supported");
                }
            }
            return cache;
        }

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
        public int CompareTo(Sample other) => this == other ? 0 : this < other ? 1 : -1;
        
        public override bool Equals(object? obj)
        {
            if(obj is Sample sample)
            {
                return Equals(sample);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                foreach (byte b in AsByteArray())
                {
                    result = (result * 31) ^ b;
                }
                return result;
            }
        }

        public bool Equals([AllowNull] Sample other) 
            => Definition == other.Definition && other.AsByteArray().SequenceEqual(AsByteArray());

        private static double LimitTo(ushort bits, double value)
        {
            var max = Math.Pow(256, bits / 8) / 2;
            var min = max * -1;
            return value < 0 ? Math.Max(min, value) : Math.Min(max, value);
        }
    }
}
