﻿using NtFreX.Audio.Infrastructure;
using System.Linq;

namespace NtFreX.Audio.Samplers
{
    internal sealed class FivePointZeroSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.FivePointZero;

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
        public override byte[] GetSideLeft(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(3 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetSideRight(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(4 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
    }
}
