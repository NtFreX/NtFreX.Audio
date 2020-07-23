using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class SurroundSampleChannelMapping : SampleChannelMapping
    {
        public override Speaker Speaker => Speaker.Surround;

        public override Sample GetFrontLeft(Sample[] sample) => sample[0];
        public override Sample GetFrontRight(Sample[] sample) => sample[1];
        public override Sample GetFrontCenter(Sample[] sample) => sample[2];
        public override Sample GetBackCenter(Sample[] sample) => sample[3];
    }
}
