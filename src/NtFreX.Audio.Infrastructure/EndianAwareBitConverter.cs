using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NtFreX.Audio.Infrastructure
{
    public static class EndianAwareBitConverter
    {
        [return: NotNull] public static byte[] ToByteArray(this float value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this double value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this long value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this int value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this short value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this sbyte value)
            => new[] { (byte)value };
        [return: NotNull] public static byte[] ToByteArray(this uint value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this ushort value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray([NotNull] this string value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(Encoding.ASCII.GetBytes(value), isLittleEndian);

        public static double ToFloat([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToSingle(switched);
        }
        public static double ToDouble([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToDouble(switched);
        }
        public static long ToInt64([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt64(switched);
        }
        public static int ToInt32([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt32(switched);
        }
        public static short ToInt16([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToInt16(switched);
        }
        public static sbyte ToInt8([NotNull] this byte[] value)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return (sbyte)value[0];
        }
        public static uint ToUInt32([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToUInt32(switched);
        }
        public static ushort ToUInt16([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return BitConverter.ToUInt16(switched);
        }
        [return:NotNull] public static string ToAscii([NotNull] this byte[] value, bool isLittleEndian = true)
            => Encoding.ASCII.GetString(value.SwitcheEndiannessWhenNotSameAsBitConverter(isLittleEndian));

        [return: NotNull] public static byte[] SwitcheEndiannessWhenNotSameAsBitConverter([NotNull] this byte[] value, bool isLittleEndian = true)
            => isLittleEndian != BitConverter.IsLittleEndian ? SwitchEndianness(value) : value;
        [return: NotNull] public static byte[] SwitchEndianness([NotNull] this byte[] value)
            => value.Reverse().ToArray();
    }
}
