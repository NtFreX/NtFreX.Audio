using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Math
{
    public static class EndianAwareBitConverter
    {
        public static byte[] ToByteArray(this int value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        public static byte[] ToByteArray(this uint value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);
        public static byte[] ToByteArray(this ushort value, bool isLittleEndian = true)
            => SwitcheEndiannessWhenNotSameAsBitConverter(BitConverter.GetBytes(value), isLittleEndian);

        /// <summary>
        /// Pads or trims the given byte array to 4 bytes and converts it with the given endianes
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static int ToInt32(this byte[] value, bool isLittleEndian = true)
        {
            /*int target = 0;
            for(int i = 0; i < value.Length; i++)
            {
                target = target | value[i];
                if (i < value.Length - 1)
                    target <<= 8;
            }*/
            var max = System.Math.Pow(256, value.Length);
            var parsed = BitConverter.ToUInt32(ToNumberRepresentation(value, 4, isLittleEndian));
            //TODO: find out why this code is nessesary!!!
            return (int) (parsed >= max / 2 ? parsed - max : parsed);
        }
        public static short ToInt16(this byte[] value, bool isLittleEndian = true)
        {
            var max = System.Math.Pow(256, value.Length);
            var parsed = BitConverter.ToUInt16(ToNumberRepresentation(value, 2, isLittleEndian));
            //TODO: find out why this code is nessesary and why the tests fail!!!
            return (short)(parsed >= max ?  parsed - max : parsed);
        }
        public static uint ToUInt32(this byte[] value, bool isLittleEndian = true)
            => BitConverter.ToUInt32(ToNumberRepresentation(value, 4, isLittleEndian));
        public static ushort ToUInt16(this byte[] value, bool isLittleEndian = true)
            => BitConverter.ToUInt16(ToNumberRepresentation(value, 2, isLittleEndian));

        public static int[] ToInt32(this IEnumerable<byte[]> value, bool isLittleEndian = true) => value.Select(x => x.ToInt32(isLittleEndian)).ToArray();
        public static int[][] ToInt32(this IEnumerable<byte[]>[] value, bool isLittleEndian = true) => value.Select(x => x.Select(y => y.ToInt32(isLittleEndian)).ToArray()).ToArray();

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
        }
        public static byte[] SwitcheEndiannessWhenNotSameAsBitConverter(this byte[] value, bool isLittleEndian = true)
            => BitConverter.IsLittleEndian != isLittleEndian ? SwitchEndianness(value) : value;
        public static byte[] SwitchEndianness(this byte[] value)
            => value.Reverse().ToArray();
    }
}
