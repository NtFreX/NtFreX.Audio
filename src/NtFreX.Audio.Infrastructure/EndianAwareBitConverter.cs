using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace NtFreX.Audio.Infrastructure
{
    public static class EndianAwareBitConverter
    {
        public static byte[] ToByteArray(this float value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian).ToArray();
        public static byte[] ToByteArray(this double value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian).ToArray();
        public static byte[] ToByteArray(this long value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian).ToArray();
        public static byte[] ToByteArray(this int value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian).ToArray();
        public static byte[] ToByteArray(this short value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian).ToArray();
        public static byte[] ToByteArray(this sbyte value)
            => new[] { (byte)(value + sbyte.MinValue) };
        public static byte[] ToByteArray(this uint value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian).ToArray();
        public static byte[] ToByteArray(this ushort value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian).ToArray();
        public static byte[] ToByteArray(this string value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(Encoding.ASCII.GetBytes(value), isLittleEndian).ToArray();

        public static double ToFloat(this Memory<byte> value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToSingle(switched.Span);
        }
        public static double ToDouble(this Memory<byte> value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToDouble(switched.Span);
        }
        public static long ToInt64(this Memory<byte> value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt64(switched.Span);
        }
        public static int ToInt32(this Memory<byte> value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt32(switched.Span);
        }

        public static short ToInt16(this Memory<byte> value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            // TODO: no pining needed?
            // TODO: if this is good like this change other methods and rename class into something like unsafe**
            return Unsafe.ReadUnaligned<short>(ref switched.Span[0]);
        }
        public static sbyte ToInt8(this Memory<byte> value)
        {
            return (sbyte) (value.Span[0] + sbyte.MinValue);
        }
        public static uint ToUInt32(this Memory<byte> value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToUInt32(switched.Span);
        }
        public static ushort ToUInt16(this Memory<byte> value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToUInt16(switched.Span);
        }
        public static string ToAscii(this Memory<byte> value, bool isLittleEndian = true)
            => Encoding.ASCII.GetString(value.SwitcheEndiannessWhenNotSameAsBitConverter(isLittleEndian).Span);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Memory<byte> SwitcheEndiannessWhenNotSameAsBitConverter(this Memory<byte> value, bool isLittleEndian = true)
            => isLittleEndian != BitConverter.IsLittleEndian ? SwitchEndianness(value) : value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SwitchEndianness(this Memory<byte> value)
            => value.ToArray().Reverse().ToArray();
    }
}
