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
        [TestCase(short.MaxValue)]
        [TestCase(short.MinValue)]
        public void CanConvertToInt32(int value)
        {
            var result = EndianAwareBitConverter.ToInt32(BitConverter.GetBytes(value));

            Assert.AreEqual(result, value);
        }
    }
}
