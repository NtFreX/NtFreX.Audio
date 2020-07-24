using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class FivePointOneSampleChannelMapping : SampleChannelMapping
    {
        public override Speakers Speaker => Speakers.FivePointOne;

        public override Sample GetFrontLeft(Sample[] sample) => sample[0];
        public override Sample GetFrontRight(Sample[] sample) => sample[1];
        public override Sample GetFrontCenter(Sample[] sample) => sample[2];
        public override Sample GetLowFrequency(Sample[] sample) => sample[3];
        public override Sample GetBackLeft(Sample[] sample) => sample[4];
        public override Sample GetBackRight(Sample[] sample) => sample[5];
    }
}
