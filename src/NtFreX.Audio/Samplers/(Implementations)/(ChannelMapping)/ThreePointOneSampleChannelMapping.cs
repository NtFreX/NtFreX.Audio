﻿using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal sealed class ThreePointOneSampleChannelMapping : SampleChannelMapping
    {
        public override Speakers Speaker => Speakers.ThreePointOne;

        public override Sample GetFrontLeft(Sample[] sample) => sample[0];
        public override Sample GetFrontRight(Sample[] sample) => sample[1];
        public override Sample GetFrontCenter(Sample[] sample) => sample[2];
        public override Sample GetLowFrequency(Sample[] sample) => sample[3];
    }
}
