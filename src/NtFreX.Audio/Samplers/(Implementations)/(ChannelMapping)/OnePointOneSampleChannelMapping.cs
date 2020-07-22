using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class OnePointOneSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.OnePointOne;

        public override Sample GetFrontCenter(Sample[] sample) => sample[0];
        public override Sample GetLowFrequency(Sample[] sample) => sample[1];
    }
}
