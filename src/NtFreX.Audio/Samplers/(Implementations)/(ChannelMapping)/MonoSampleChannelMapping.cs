using NtFreX.Audio.Infrastructure;

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
