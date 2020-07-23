using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class TwoPointOneSampleChannelMapping : SampleChannelMapping
    {
        public override Speakers Speaker => Speakers.TwoPointOne;

        public override Sample GetFrontLeft(Sample[] sample) => sample[0];
        public override Sample GetFrontRight(Sample[] sample) => sample[1];
        public override Sample GetLowFrequency(Sample[] sample) => sample[2];
    }
}
