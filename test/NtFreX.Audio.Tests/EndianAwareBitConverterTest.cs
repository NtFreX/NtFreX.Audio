using NtFreX.Audio.Infrastructure;
using NUnit.Framework;
using System;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class EndianAwareBitConverterTest
    {
        [TestCase(0, -128)]
        [TestCase(10, -118)]
        [TestCase(1, -127)]
        [TestCase(100, -28)]
        [TestCase(200, 72)]
        [TestCase(byte.MaxValue, sbyte.MaxValue)]
        [TestCase(byte.MinValue, sbyte.MinValue)]
        public void CanConvertToInt8(int value, int expecedResult)
        {
            var result = EndianAwareBitConverter.ToInt8(new byte[] { (byte)value });

            Assert.AreEqual(expecedResult, result);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(250)]
        [TestCase(short.MaxValue)]
        [TestCase(short.MinValue)]
        public void CanConvertToInt16(short value)
        {
            var result = EndianAwareBitConverter.ToInt16(BitConverter.GetBytes(value));

            Assert.AreEqual(result, value);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(250)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        public void CanConvertToInt32(int value)
        {
            var result = EndianAwareBitConverter.ToInt32(BitConverter.GetBytes(value));

            Assert.AreEqual(result, value);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(250)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        public void CanConvertToInt64(long value)
        {
            var result = EndianAwareBitConverter.ToInt64(BitConverter.GetBytes(value));

            Assert.AreEqual(result, value);
        }
    }
}
