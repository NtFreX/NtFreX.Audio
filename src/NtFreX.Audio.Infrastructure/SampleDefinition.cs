using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Infrastructure
{
    public struct SampleDefinition : IEquatable<SampleDefinition>
    {
        public AudioFormatType Type { get; }
        public ushort Bits { get; }
        public bool IsLittleEndian { get; }
        public ushort Bytes => (ushort) (Bits / 8);

        public SampleDefinition(AudioFormatType type, ushort bits, bool isLittleEndian)
        {
            Type = type;
            Bits = bits;
            IsLittleEndian = isLittleEndian;
        }

        public override bool Equals(object? obj)
        {
            if(obj is SampleDefinition definition)
            {
                return Equals(definition);
            }
            return false;
        }

        public bool Equals([AllowNull] SampleDefinition other)
            => Type == other.Type && Bits == other.Bits && IsLittleEndian == other.IsLittleEndian;

        public override int GetHashCode() => HashCode.Combine(Type, Bits, IsLittleEndian);

        public static bool operator ==(SampleDefinition left, SampleDefinition right) => left.Equals(right);
        public static bool operator !=(SampleDefinition left, SampleDefinition right) => !(left == right);
    }
}
