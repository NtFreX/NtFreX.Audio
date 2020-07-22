using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    internal abstract class SampleChannelMapping
    {
        public abstract Speaker Speaker { get; }

        protected SampleChannelMapping() { }

        public virtual Sample GetFrontLeft(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetFrontRight(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetFrontCenter(Sample[] sample)
        {
            //TODO: fix overflow for 64bit audio
            return (GetFrontLeft(sample) + GetFrontRight(sample)) / 2;
        }
        public virtual Sample GetLowFrequency(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetBackLeft(Sample[] sample) => GetFrontLeft(sample);
        public virtual Sample GetBackRight(Sample[] sample) => GetFrontRight(sample);
        public virtual Sample GetFrontLeftOfCenter(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetFrontRightOfCenter(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetBackCenter(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetSideLeft(Sample[] sample) => GetFrontLeft(sample);
        public virtual Sample GetSideRight(Sample[] sample) => GetFrontRight(sample);
        public virtual Sample GetTopCenter(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetTopLeft(Sample[] sample) => GetFrontLeft(sample);
        public virtual Sample GetTopRight(Sample[] sample) => GetFrontRight(sample);
        public virtual Sample GetTopFrontLeft(Sample[] sample) => GetFrontLeft(sample);
        public virtual Sample GetTopFrontCenter(Sample[] sample) => GetFrontCenter(sample);
        public virtual Sample GetTopFrontRight(Sample[] sample) => GetFrontRight(sample);
        public virtual Sample GetTopBackLeft(Sample[] sample) => GetBackLeft(sample);
        public virtual Sample GetTopBackCenter(Sample[] sample) => GetBackCenter(sample);
        public virtual Sample GetTopBackRight(Sample[] sample) => GetBackRight(sample);
       
        public virtual Sample[] ToMono(Sample[] sample)
        {
            //TODO: OVERFLOW!!!!!!
            var total = GetFrontLeft(sample) +
               GetFrontRight(sample) +
               GetFrontCenter(sample) +
               GetLowFrequency(sample) +
               GetBackLeft(sample) +
               GetBackRight(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetSideRight(sample) +
               GetTopCenter(sample) +
               GetTopFrontLeft(sample) +
               GetTopFrontRight(sample) +
               GetTopBackLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample);

            return new Sample[] { total / 17 };
        }
        public virtual Sample[] ToOnePointOne(Sample[] sample)
        {
            var center = (GetFrontLeft(sample) +
               GetFrontRight(sample) +
               GetFrontCenter(sample) +
               GetBackLeft(sample) +
               GetBackRight(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetSideRight(sample) +
               GetTopCenter(sample) +
               GetTopFrontLeft(sample) +
               GetTopFrontRight(sample) +
               GetTopBackLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 17;

            return new Sample[] { center, GetLowFrequency(sample) };
        }
        public virtual Sample[] ToStereo(Sample[] sample)
        {
            var left = (GetFrontLeft(sample) +
               GetFrontCenter(sample) +
               GetLowFrequency(sample) +
               GetBackLeft(sample) +
               GetFrontLeftOfCenter(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopCenter(sample) +
               GetTopFrontLeft(sample) +
               GetTopBackLeft(sample) +
               GetTopBackCenter(sample)) / 11;

            var right = (GetFrontRight(sample) +
               GetFrontCenter(sample) +
               GetLowFrequency(sample) +
               GetBackRight(sample) +
               GetFrontRightOfCenter(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopCenter(sample) +
               GetTopFrontRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 11;

            return new Sample[] { left, right };
        }
        public virtual Sample[] ToTwoPointOne(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample) +
               GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample) +
               GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 12;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample) +
               GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample) +
               GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 12;

            return new Sample[] { frontLeft, frontRight, GetLowFrequency(sample) };
        }
        public virtual Sample[] ToThreePointZero(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample) +
               GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 7;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample) +
               GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 7;

            var frontCenter = (GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 5;

            return new Sample[] { frontLeft, frontRight, frontCenter };
        }
        public virtual Sample[] ToThreePointOne(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample) +
               GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 7;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample) +
               GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 7;

            var frontCenter = (GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 5;

            return new Sample[] { frontLeft, frontRight, frontCenter, GetLowFrequency(sample) };
        }
        public virtual Sample[] ToQuad(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample) +
               GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 7;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample) +
               GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 7;

            var backLeft = (GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 5;

            var backRight = (GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 5;

            return new Sample[] { frontLeft, frontRight, backLeft, backRight };
        }
        public virtual Sample[] ToSurround(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample)) / 2;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample)) / 2;

            var frontCenter = (GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 5;

            var backCenter = (GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample) +
               GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 10;

            return new Sample[] { frontLeft, frontRight, frontCenter, backCenter };
        }
        public virtual Sample[] ToFivePointZero(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
              GetTopFrontLeft(sample)) / 2;

            var frontRight = (GetFrontRight(sample) +
              GetTopFrontRight(sample)) / 2;

            var frontCenter = (GetFrontCenter(sample) +
              GetFrontLeftOfCenter(sample) +
              GetFrontRightOfCenter(sample) +
              GetTopCenter(sample) +
              GetTopFrontCenter(sample)) / 5;

            var sideLeft = (GetBackLeft(sample) +
              GetBackCenter(sample) +
              GetSideLeft(sample) +
              GetTopBackCenter(sample) +
              GetTopBackLeft(sample)) / 5;

            var sideRight = (GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 5;

            return new Sample[] { frontLeft, frontRight, frontCenter, sideLeft, sideRight };
        }
        public virtual Sample[] ToFivePointOne(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample)) / 2;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample)) / 2;

            var frontCenter = (GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 5;

            var backLeft = (GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 5;

            var backRight = (GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 5;

            return new Sample[] { frontLeft, frontRight, frontCenter, GetLowFrequency(sample), backLeft, backRight };
        }
        public virtual Sample[] ToSevenPointZero(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample)) / 2;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample)) / 2;

            var frontCenter = (GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 5;

            var backLeft = (GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 4;

            var backRight = (GetBackRight(sample) +
               GetBackCenter(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 4;

            return new Sample[] { frontLeft, frontRight, frontCenter, backLeft, backRight, GetSideLeft(sample), GetSideRight(sample) };
        }
        public virtual Sample[] ToSevenPointOne(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample)) / 2;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample)) / 2;

            var frontCenter = (GetFrontCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 3;

            var backLeft = (GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 5;

            var backRight = (GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 5;

            return new Sample[] { frontLeft, frontRight, frontCenter, GetLowFrequency(sample), backLeft, backRight, GetFrontLeftOfCenter(sample), GetFrontRightOfCenter(sample) };
        }
     
        public virtual Sample[] ToFivePointOneSurround(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample)) / 2;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample)) / 2;

            var frontCenter = (GetFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample)) / 5;

            var sideLeft = (GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetSideLeft(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 5;

            var sideRight = (GetBackRight(sample) +
               GetBackCenter(sample) +
               GetSideRight(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 5;

            return new Sample[] { frontLeft, frontRight, frontCenter, GetLowFrequency(sample), sideLeft, sideRight };
        }
        public virtual Sample[] ToSevenPointOneSurround(Sample[] sample)
        {
            var frontLeft = (GetFrontLeft(sample) +
               GetTopFrontLeft(sample)) / 2;

            var frontRight = (GetFrontRight(sample) +
               GetTopFrontRight(sample)) / 2;

            var frontCenter = (GetFrontCenter(sample) +
               GetTopCenter(sample) +
               GetTopFrontCenter(sample) +
               GetFrontLeftOfCenter(sample) +
               GetFrontRightOfCenter(sample)) / 5;

            var backLeft = (GetBackLeft(sample) +
               GetBackCenter(sample) +
               GetTopBackCenter(sample) +
               GetTopBackLeft(sample)) / 4;

            var backRight = (GetBackRight(sample) +
               GetBackCenter(sample) +
               GetTopBackCenter(sample) +
               GetTopBackRight(sample)) / 4;

            return new Sample[] { frontLeft, frontRight, frontCenter, GetLowFrequency(sample), backLeft, backRight, GetSideLeft(sample), GetSideRight(sample) };
        }
    }
}
