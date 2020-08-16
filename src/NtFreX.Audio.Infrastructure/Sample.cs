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
            this.Value = NumberFactory.ConstructNumber(definition.Type, definition.IsLittleEndian, value);
        }

        public Sample(double value, SampleDefinition definition)
        {
            Definition = definition;

            this.cache = null;
            this.Value = value;
        }

        public static Sample Zero(SampleDefinition definition) => new Sample(0d, definition);

        public static Sample operator +(Sample a, Sample b)
            => a.Definition == b.Definition ? new Sample(a.Value + b.Value, a.Definition) : throw new Exception();
        public static Sample operator -(Sample a, Sample b)
            => a.Definition == b.Definition ? new Sample(a.Value - b.Value, a.Definition) : throw new Exception();
        public static Sample operator +(Sample a, double b)
            => new Sample(a.Value + b, a.Definition);
        public static Sample operator -(Sample a, double b)
            => new Sample(a.Value - b, a.Definition);
        public static Sample operator /(Sample a, double b)
            => new Sample(a.Value / b, a.Definition);
        public static Sample operator *(Sample a, double b)
            => new Sample(a.Value * b, a.Definition);
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
                cache = NumberFactory.DeconstructNumber(Definition, Value);
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
    }
}
