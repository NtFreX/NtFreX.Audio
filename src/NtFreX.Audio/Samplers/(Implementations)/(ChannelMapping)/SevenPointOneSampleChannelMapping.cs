using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NtFreX.Audio.Samplers
{
    internal sealed class SevenPointOneSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.SevenPointOne;

        public override byte[] GetFrontLeft(byte[] sample, ushort bitsPerSample)
        {
            return sample.Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetFrontRight(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(1 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetFrontCenter(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(2 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetLowFrequency(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(3 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetBackLeft(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(4 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetBackRight(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(5 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetFrontLeftOfCenter(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(6 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetFrontRightOfCenter(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(7 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
    }
}
