using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{

    internal sealed class StereoSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => throw new NotImplementedException();

        public override byte[] GetFrontLeft(byte[] sample, ushort bitsPerSample)
        {
            return sample.Take(bitsPerSample / 8).ToArray();
        }
        public override byte[] GetFrontRight(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(bitsPerSample / 8).ToArray();
        }

        public override byte[] ToMono(byte[] sample, ushort bitsPerSample)
        {
            var total = GetFrontLeft(sample, bitsPerSample).ToInt64() +
                GetFrontRight(sample, bitsPerSample).ToInt64();

            return (total / 2).ToByteArray(bitsPerSample);
        }
    }
}
