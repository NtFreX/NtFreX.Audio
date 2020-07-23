using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{

    internal sealed class MonoSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.Mono;
        public override Sample GetFrontCenter(Sample[] sample) => sample[0];
    }
}
