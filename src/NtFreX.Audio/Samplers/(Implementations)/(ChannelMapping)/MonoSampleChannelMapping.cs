using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class MonoSampleChannelMapping : SampleChannelMapping
    {
        public override Speakers Speaker => Speakers.Mono;
        public override Sample GetFrontCenter(Sample[] sample) => sample[0];
    }
}
