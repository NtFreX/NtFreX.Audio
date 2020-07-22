using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{

    internal sealed class StereoSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.Stereo;

        public override Sample GetFrontLeft(Sample[] sample) => sample[0];
        public override Sample GetFrontRight(Sample[] sample) => sample[1];

        public override Sample[] ToMono(Sample[] sample)
        {
            var total = GetFrontLeft(sample) + GetFrontRight(sample);

            return new Sample[] { total / 2 };
        }
    }
}
