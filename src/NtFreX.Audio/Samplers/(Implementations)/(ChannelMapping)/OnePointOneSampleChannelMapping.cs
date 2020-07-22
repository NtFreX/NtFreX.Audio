using NtFreX.Audio.Infrastructure;
using System.Linq;

namespace NtFreX.Audio.Samplers
{
    internal sealed class OnePointOneSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.OnePointOne;

        public override byte[] GetFrontCenter(byte[] sample, ushort bitsPerSample)
        {
            return sample.Take(bitsPerSample / 8).ToArray();
        }

        public override byte[] GetLowFrequency(byte[] sample, ushort bitsPerSample)
        {
            return sample.Skip(1 * bitsPerSample / 8).Take(bitsPerSample / 8).ToArray();
        }
    }
}
