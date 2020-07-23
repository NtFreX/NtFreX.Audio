using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class QuadSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.Quad;

        public override Sample GetFrontLeft(Sample[] sample) => sample[0];
        public override Sample GetFrontRight(Sample[] sample) => sample[1];
        public override Sample GetBackLeft(Sample[] sample) => sample[2];
        public override Sample GetBackRight(Sample[] sample) => sample[3];
    }
}
