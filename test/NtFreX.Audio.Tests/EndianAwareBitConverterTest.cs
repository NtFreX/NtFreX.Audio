using NtFreX.Audio.Math;
using NUnit.Framework;
using System;

namespace NtFreX.Audio.Tests
{
    [TestFixture]
    public class EndianAwareBitConverterTest
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(sbyte.MaxValue)]
        [TestCase(sbyte.MinValue)]
        public void CanConvertFromInt8ToInt64(sbyte value)
        {
            var data = BitConverter.GetBytes(value);
            var result = EndianAwareBitConverter.ToInt64(data);

            Assert.AreEqual(result, value);
        }


        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(sbyte.MaxValue)]
        [TestCase(sbyte.MinValue)]
        public void CanConvertToInt8(sbyte value)
        {
            var result = EndianAwareBitConverter.ToInt8(BitConverter.GetBytes(value));

            Assert.AreEqual(result, value);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(250)]
        [TestCase(Int16.MaxValue)]
        [TestCase(Int16.MinValue)]
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
        [TestCase(Int32.MaxValue)]
        [TestCase(Int32.MinValue)]
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
        [TestCase(Int64.MaxValue)]
        [TestCase(Int64.MinValue)]
        public void CanConvertToInt64(long value)
        {
            var result = EndianAwareBitConverter.ToInt64(BitConverter.GetBytes(value));

            Assert.AreEqual(result, value);
        }
    }
}
