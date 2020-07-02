using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Extensions
{
    // TODO: cleanup/use arrays or span
    public static class ByteEnumerableExtensions
    {
        public static int[] ToInt(this IEnumerable<byte[]> value, bool isLittleEndian = true) => value.Select((x) => x.ToInt(isLittleEndian)).ToArray();
        public static int[][] ToInt(this IEnumerable<byte[]>[] value, bool isLittleEndian = true) => value.Select((x) => x.ToInt(isLittleEndian).ToArray()).ToArray();

        /// <summary>
        /// Pads or trims the given byte array to 4 bytes and converts it with the given endianes
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        public static int ToInt(this byte[] value, bool isLittleEndian = true)
        {
            var umax = (int)System.Math.Pow(256, value.Length);
            var max = umax / 2;
            if (BitConverter.IsLittleEndian && !isLittleEndian || !BitConverter.IsLittleEndian && isLittleEndian)
            {
                value.Reverse().ToArray().ToInt(BitConverter.IsLittleEndian);
            }

            int result;
            if (BitConverter.IsLittleEndian)
            {
                result = BitConverter.ToInt32(value.PadOrTrimRight(4).ToArray());
            }
            else
            {
                result = BitConverter.ToInt32(value.PadOrTrimLeft(4).ToArray());
            }

            return result >= max ? result - umax : result;
        }

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
    }
}
