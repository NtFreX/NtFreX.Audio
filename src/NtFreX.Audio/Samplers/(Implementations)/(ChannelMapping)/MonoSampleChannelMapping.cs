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

    internal sealed class MonoSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.Mono;
        public override byte[] GetFrontCenter(byte[] sample, ushort bitsPerSample)
        {
            return sample;
        }
    }
}
