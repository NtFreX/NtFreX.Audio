using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class StereoSampleChannelMapping : SampleChannelMapping
    {
        public override Speakers Speaker => Speakers.Stereo;

        public override Sample GetFrontLeft(Sample[] sample) => sample[0];
        public override Sample GetFrontRight(Sample[] sample) => sample[1];

        public override Sample[] ToMono(Sample[] sample)
        {
            var center = new Sample[] { GetFrontLeft(sample), GetFrontRight(sample) }.Average();

            return new Sample[] { center };
        }
    }
}
