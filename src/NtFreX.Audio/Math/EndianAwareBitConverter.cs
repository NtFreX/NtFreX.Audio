using NtFreX.Audio.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NtFreX.Audio.Math
{
    public static class EndianAwareBitConverter
    {
        [return: NotNull] public static byte[] ToByteArray(this long value, int targetLength, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(ParseByLength(value, targetLength), isLittleEndian);

        [return: NotNull] public static byte[] ToByteArray(this int value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this short value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this uint value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray(this ushort value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        [return: NotNull] public static byte[] ToByteArray([NotNull] this string value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(Encoding.ASCII.GetBytes(value), isLittleEndian);

        public static long ToInt64([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return ParseByLength(value, isLittleEndian);
        }
        public static int ToInt32([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return (int) ParseByLength(value, isLittleEndian);
        }
        public static sbyte ToInt8([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return (sbyte)ParseByLength(value, isLittleEndian);
        }
        public static short ToInt16([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return (short) ParseByLength(value, isLittleEndian);
        }
        public static uint ToUInt32([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return (uint)ParseByLength(value, isLittleEndian);
        }
        public static ushort ToUInt16([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));

            return (ushort)ParseByLength(value, isLittleEndian);
        }

        [return:NotNull] public static string ToAscii([NotNull] this byte[] value, bool isLittleEndian = true)
            => Encoding.ASCII.GetString(value.SwitcheEndiannessWhenNotSameAsBitConverter(isLittleEndian));

        [return: NotNull] public static int[] ToInt32([NotNull] this IEnumerable<byte[]> value, bool isLittleEndian = true) => value.Select(x => x.ToInt32(isLittleEndian)).ToArray();
        [return: NotNull] public static int[][] ToInt32([NotNull] this IEnumerable<byte[]>[] value, bool isLittleEndian = true) => value.Select(x => x.Select(y => y.ToInt32(isLittleEndian)).ToArray()).ToArray();
        
        [return: NotNull] public static byte[] SwitcheEndiannessWhenNotSameAsBitConverter([NotNull] this byte[] value, bool isLittleEndian = true)
            => isLittleEndian != BitConverter.IsLittleEndian ? SwitchEndianness(value) : value;

        [return: NotNull] public static byte[] SwitchEndianness([NotNull] this byte[] value)
            => value.Reverse().ToArray();

        // TODO: make this stuff work
        [return: NotNull]
        private static byte[] ParseByLength(this long value, int targetLength)
        {
            return targetLength switch
            {
                1 => new[] { (byte)value }, //new[] { value >= 0 ? (byte) (value + 128) : (byte)((128 + value) * -1) },
                2 => BitConverter.GetBytes((short)value),
                4 => BitConverter.GetBytes((int)value),
                8 => BitConverter.GetBytes(value),
                _ => throw new ArgumentException(ExceptionMessages.TargetLengthNotSupported, nameof(targetLength)),
            };
        }
        private static long ParseByLength([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            return value.Length switch
            {
                1 => (sbyte)value[0], //value[0] < 128 ? (128 - value[0]) * -1 : value[0] - 128,
                2 => BitConverter.ToInt16(switched),
                4 => BitConverter.ToInt32(switched),
                8 => BitConverter.ToInt64(switched),
                _ => throw new ArgumentException(ExceptionMessages.ArrayLengthNotSupported, nameof(value)),
            };
        }
    }
}
