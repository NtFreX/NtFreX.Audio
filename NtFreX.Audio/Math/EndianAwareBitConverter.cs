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

        // TODO: make this stuff work
        [return: NotNull] private static byte[] ParseByLength(this long value, int targetLength)
        {
            switch (targetLength)
            {
                case 1: return new [] { (byte)value };
                case 2: return BitConverter.GetBytes((short)value);
                case 4: return BitConverter.GetBytes((int) value);
                case 8: return BitConverter.GetBytes(value);
                default: throw new ArgumentException();
            }
        }
        private static long ParseByLength([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            var switched = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            switch (value.Length)
            {
                case 1: return value[0];
                case 2: return BitConverter.ToInt16(switched);
                case 4: return BitConverter.ToInt32(switched);
                case 8: return BitConverter.ToInt64(switched);
                default: throw new ArgumentException();
            }
        }

        public static long ToInt64([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            return ParseByLength(value, isLittleEndian);
        }
        public static int ToInt32([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            /*int target = 0;
            for(int i = 0; i < value.Length; i++)
            {
                target = target | value[i];
                if (i < value.Length - 1)
                    target <<= 8;
            }*/
            return (int) ParseByLength(value, isLittleEndian); //BitConverter.ToInt32(TwoComplementToSingle(ToNumberRepresentation(value, 4, isLittleEndian)));
            //return (int)Overflow(parsed, value.Length);
        }
        public static short ToInt16([NotNull] this byte[] value, bool isLittleEndian = true)
        {
            return (short) ParseByLength(value, isLittleEndian);
            //return (short) Overflow(parsed, value.Length);
        }
        public static uint ToUInt32([NotNull] this byte[] value, bool isLittleEndian = true)
            => (uint) ParseByLength(value, isLittleEndian);
        public static ushort ToUInt16([NotNull] this byte[] value, bool isLittleEndian = true)
            => (ushort) ParseByLength(value, isLittleEndian);

        [return:NotNull] public static string ToAscii([NotNull] this byte[] value, bool isLittleEndian = true)
            => Encoding.ASCII.GetString(value.SwitcheEndiannessWhenNotSameAsBitConverter(isLittleEndian));

        [return: NotNull] public static int[] ToInt32([NotNull] this IEnumerable<byte[]> value, bool isLittleEndian = true) => value.Select(x => x.ToInt32(isLittleEndian)).ToArray();
        [return: NotNull] public static int[][] ToInt32([NotNull] this IEnumerable<byte[]>[] value, bool isLittleEndian = true) => value.Select(x => x.Select(y => y.ToInt32(isLittleEndian)).ToArray()).ToArray();
        /*
        public static IEnumerable<byte> PadOrTrimLeft(this IEnumerable<byte> value, int length)
        {
            var resolvedValues = value.ToArray();
            if (resolvedValues.Length >= length)
            {
                foreach (var data in resolvedValues)
                {
                    yield return data;
                }
            }

            for (int i = 0; i < length - resolvedValues.Length; i++)
            {
                yield return 0;
            }

            foreach (var data in resolvedValues)
            {
                yield return data;
            }
        }
        public static IEnumerable<byte> PadOrTrimRight(this IEnumerable<byte> value, int length)
        {
            var enumerator = value.GetEnumerator();
            for (var i = 0; i < length; i++)
            {
                if (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                else
                {
                    yield return 0;
                }
            }
        }

        public static byte[] ToNumberRepresentation(this byte[] value, int length, bool isLittleEndian = true)
        {
            var endianValue = SwitcheEndiannessWhenNotSameAsBitConverter(value, isLittleEndian);
            var padded = BitConverter.IsLittleEndian
                ? endianValue.PadOrTrimRight(length)
                : endianValue.PadOrTrimLeft(length);
            return padded.ToArray();
        }*/

        [return: NotNull] public static byte[] SwitcheEndiannessWhenNotSameAsBitConverter([NotNull] this byte[] value, bool isLittleEndian = true)
            => BitConverter.IsLittleEndian != isLittleEndian ? SwitchEndianness(value) : value;

        [return: NotNull] public static byte[] SwitchEndianness([NotNull] this byte[] value)
            => value.Reverse().ToArray();

        //TODO: find out why this code is nessesary!!! do same when converting back
        /*private static double Overflow(uint value, int length)
        {
            var max = System.Math.Pow(256, length);
            return value >= max / 2 ? value - max : value;
        }*/
        /*

        private static byte[] SingleToTwoComplement(byte[] value)
            => TwoComplementToSingle(value);
        private static byte[] TwoComplementToSingle(byte[] value)
        {
            var start = value.Length / 2;
            for (var i = start; i < value.Length; i++)
            {
                value[i] |= value[i];
            }
            return value;
        }*/
    }
}
