using System;
using System.Linq;
using System.Text;

namespace NtFreX.Audio.Infrastructure
{
    public static class EndianAwareBitConverter
    {
        public static Memory<byte> ToMemory(this float value, bool isLittleEndian = true)
        {
            var mem = BitConverter.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }
        public static Memory<byte> ToMemory(this double value, bool isLittleEndian = true)
        {
            var mem = BitConverter.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }
        public static Memory<byte> ToMemory(this long value, bool isLittleEndian = true)
        {
            var mem = BitConverter.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }
        public static Memory<byte> ToMemory(this int value, bool isLittleEndian = true)
        {
            var mem = BitConverter.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }
        public static Memory<byte> ToMemory(this short value, bool isLittleEndian = true)
        {
            var mem = BitConverter.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }
        public static Memory<byte> ToMemory(this sbyte value)
            => new[] { (byte)(value + sbyte.MinValue) };
        public static Memory<byte> ToMemory(this uint value, bool isLittleEndian = true)
        {
            var mem = BitConverter.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }
        public static Memory<byte> ToMemory(this ushort value, bool isLittleEndian = true)
        {
            var mem = BitConverter.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }
        public static Memory<byte> ToMemory(this string value, bool isLittleEndian = true)
        {
            var mem = Encoding.ASCII.GetBytes(value);
            SwitcheEndiannessWhenNotSameAsBitConverter(mem, isLittleEndian);
            return mem;
        }

        public static byte[] ToByteArray(this float value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();
        public static byte[] ToByteArray(this double value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();
        public static byte[] ToByteArray(this long value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();
        public static byte[] ToByteArray(this int value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();
        public static byte[] ToByteArray(this short value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();
        public static byte[] ToByteArray(this sbyte value)
            => ToMemory(value).ToArray();
        public static byte[] ToByteArray(this uint value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();
        public static byte[] ToByteArray(this ushort value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();
        public static byte[] ToByteArray(this string value, bool isLittleEndian = true)
            => ToMemory(value, isLittleEndian).ToArray();

        public static double ToFloat(this Memory<byte> value, bool isLittleEndian = true)
        {
            SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToSingle(value.Span);
        }
        public static double ToDouble(this Memory<byte> value, bool isLittleEndian = true)
        {
            SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToDouble(value.Span);
        }
        public static long ToInt64(this Memory<byte> value, bool isLittleEndian = true)
        {
            SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt64(value.Span);
        }
        public static int ToInt32(this Memory<byte> value, bool isLittleEndian = true)
        {
            SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt32(value.Span);
        }
        public static short ToInt16(this Memory<byte> value, bool isLittleEndian = true)
        {
            SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt16(value.Span);
        }
        public static sbyte ToInt8(this Memory<byte> value)
        {
            return (sbyte) (value.Span[0] + sbyte.MinValue);
        }
        public static uint ToUInt32(this Memory<byte> value, bool isLittleEndian = true)
        {
            SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToUInt32(value.Span);
        }
        public static ushort ToUInt16(this Memory<byte> value, bool isLittleEndian = true)
        {
            SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToUInt16(value.Span);
        }
        public static string ToAscii(this Memory<byte> value, bool isLittleEndian = true)
        {
            value.SwitcheEndiannessWhenNotSameAsBitConverter(isLittleEndian);
            return Encoding.ASCII.GetString(value.Span);
        }

        public static void SwitcheEndiannessWhenNotSameAsBitConverter(this Memory<byte> value, bool isLittleEndian = true)
        {
            if (isLittleEndian != BitConverter.IsLittleEndian)
            {
                SwitchEndianness(value);
            }
        }

        public static void SwitchEndianness(this Memory<byte> value)
            => value.Span.Reverse();
    }
}
