using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace NtFreX.Audio.Infrastructure
{
    public struct Sample : IEquatable<Sample>
    {
        public SampleDefinition Definition { get; }

        private Memory<byte>? memory;
        private double? number;

        public Sample(Memory<byte> value, SampleDefinition definition)
        {
            Definition = definition;

            this.memory = value;
            this.number = null;
        }

        public Sample(double value, SampleDefinition definition)
        {
            Definition = definition;

            this.number = value;
            this.memory = null;
        }

        public static Sample Zero(SampleDefinition definition) => new Sample(0d, definition);

        public static Sample operator +(Sample a, Sample b)
            => a.Definition == b.Definition ? new Sample(a.AsNumber() + b.AsNumber(), a.Definition) : throw new Exception();
        public static Sample operator -(Sample a, Sample b)
            => a.Definition == b.Definition ? new Sample(a.AsNumber() - b.AsNumber(), a.Definition) : throw new Exception();
        public static Sample operator +(Sample a, double b)
            => new Sample(a.AsNumber() + b, a.Definition);
        public static Sample operator -(Sample a, double b)
            => new Sample(a.AsNumber() - b, a.Definition);
        public static Sample operator /(Sample a, double b)
            => new Sample(a.AsNumber() / b, a.Definition);
        public static Sample operator *(Sample a, double b)
            => new Sample(a.AsNumber() * b, a.Definition);
        public static bool operator <(Sample a, Sample b) => a.Definition == b.Definition ? a.AsNumber() < b.AsNumber() : throw new Exception();
        public static bool operator >(Sample a, Sample b) => a.Definition == b.Definition ? a.AsNumber() > b.AsNumber() : throw new Exception();
        public static bool operator ==(Sample left, Sample right) => left.Equals(right);
        public static bool operator !=(Sample left, Sample right) => !(left == right);

        public static Sample Add(Sample left, Sample right) => left + right;
        public static Sample Subtract(Sample left, Sample right) => left - right;
        public static Sample Divide(Sample left, double right) => left / right;
        public static Sample Multiply(Sample left, double right) => left * right;

        public double AsNumber()
        {
            if(number == null)
            {
                Debug.Assert(memory != null, "If the number is not provided memory must be");
                number = NumberFactory.ConstructNumber(Definition.Type, Definition.IsLittleEndian, memory!.Value);
            }

            return number.Value;
        }
        public Memory<byte> AsByteArray()
        {
            if (memory == null)
            {
                Debug.Assert(number != null, "If memory is not provided the number must be");
                memory = NumberFactory.DeconstructNumber(Definition, number!.Value);
            }

            return memory.Value;
        }

        public override string ToString() => AsNumber().ToString(CultureInfo.InvariantCulture);
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
                foreach (byte b in AsByteArray().Span)
                {
                    result = (result * 31) ^ b;
                }
                return result;
            }
        }

        public bool Equals([AllowNull] Sample other) 
            => Definition == other.Definition && other.AsByteArray().Span.SequenceEqual(AsByteArray().Span);
    }
}
