using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{

    internal abstract class SampleChannelMapping
    {
        public abstract Speaker Speaker { get; }

        protected SampleChannelMapping() { }

        public virtual byte[] GetFrontLeft(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetFrontRight(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetFrontCenter(byte[] sample, ushort bitsPerSample)
        {
            //TODO: fix overflow for 64bit audio
            return ((GetFrontLeft(sample, bitsPerSample).ToInt64() + GetFrontRight(sample, bitsPerSample).ToInt64()) / 2).ToByteArray(bitsPerSample / 8);

        }
        public virtual byte[] GetLowFrequency(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetBackLeft(byte[] sample, ushort bitsPerSample) => GetFrontLeft(sample, bitsPerSample);
        public virtual byte[] GetBackRight(byte[] sample, ushort bitsPerSample) => GetFrontRight(sample, bitsPerSample);
        public virtual byte[] GetFrontLeftOfCenter(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetFrontRightOfCenter(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetBackCenter(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetSideLeft(byte[] sample, ushort bitsPerSample) => GetFrontLeft(sample, bitsPerSample);
        public virtual byte[] GetSideRight(byte[] sample, ushort bitsPerSample) => GetFrontRight(sample, bitsPerSample);
        public virtual byte[] GetTopCenter(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetTopLeft(byte[] sample, ushort bitsPerSample) => GetFrontLeft(sample, bitsPerSample);
        public virtual byte[] GetTopRight(byte[] sample, ushort bitsPerSample) => GetFrontRight(sample, bitsPerSample);
        public virtual byte[] GetTopFrontLeft(byte[] sample, ushort bitsPerSample) => GetFrontLeft(sample, bitsPerSample);
        public virtual byte[] GetTopFrontCenter(byte[] sample, ushort bitsPerSample) => GetFrontCenter(sample, bitsPerSample);
        public virtual byte[] GetTopFrontRight(byte[] sample, ushort bitsPerSample) => GetFrontRight(sample, bitsPerSample);
        public virtual byte[] GetTopBackLeft(byte[] sample, ushort bitsPerSample) => GetBackLeft(sample, bitsPerSample);
        public virtual byte[] GetTopBackCenter(byte[] sample, ushort bitsPerSample) => GetBackCenter(sample, bitsPerSample);
        public virtual byte[] GetTopBackRight(byte[] sample, ushort bitsPerSample) => GetBackRight(sample, bitsPerSample);
       
        public virtual byte[] ToMono(byte[] sample, ushort bitsPerSample)
        {
            //TODO: OVERFLOW!!!!!!
            var total = GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetLowFrequency(sample, bitsPerSample).ToInt64() +
               GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackRight(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64();

            return (total / 17).ToByteArray(bitsPerSample / 8);
        }
        public virtual byte[] ToOnePointOne(byte[] sample, ushort bitsPerSample)
        {
            var center = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackRight(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 17;

            return center.ToByteArray(bitsPerSample / 8)
                .Concat(GetLowFrequency(sample, bitsPerSample))
                .ToArray();
        }
        public virtual byte[] ToStereo(byte[] sample, ushort bitsPerSample)
        {
            var left = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetLowFrequency(sample, bitsPerSample).ToInt64() +
               GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64()) / 11;

            var right = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetLowFrequency(sample, bitsPerSample).ToInt64() +
               GetBackRight(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 11;

            return left.ToByteArray(bitsPerSample / 8).Concat(right.ToByteArray(bitsPerSample / 8)).ToArray();
        }
        public virtual byte[] ToTwoPointOne(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64() +
               GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 12;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64() +
               GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 12;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(GetLowFrequency(sample, bitsPerSample))
                .ToArray();
        }
        public virtual byte[] ToThreePointZero(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64() +
               GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 7;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64() +
               GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 7;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 5;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .ToArray();
        }
        public virtual byte[] ToThreePointOne(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64() +
               GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 7;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64() +
               GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 7;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 5;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(GetLowFrequency(sample, bitsPerSample))
                .ToArray();
        }
        public virtual byte[] ToQuad(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 7;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64() +
               GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 7;

            var backLeft = (GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 5;

            var backRight = (GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 5;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(backLeft.ToByteArray(bitsPerSample / 8))
                .Concat(backRight.ToByteArray(bitsPerSample / 8))
                .ToArray();
        }
        public virtual byte[] ToSurround(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64()) / 2;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64()) / 2;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 5;

            var backCenter = (GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 10;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(backCenter.ToByteArray(bitsPerSample / 8))
                .ToArray();
        }
        public virtual byte[] ToFivePointZero(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
              GetTopFrontLeft(sample, bitsPerSample).ToInt64()) / 2;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
              GetTopFrontRight(sample, bitsPerSample).ToInt64()) / 2;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
              GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
              GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
              GetTopCenter(sample, bitsPerSample).ToInt64() +
              GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 5;

            var sideLeft = (GetBackLeft(sample, bitsPerSample).ToInt64() +
              GetBackCenter(sample, bitsPerSample).ToInt64() +
              GetSideLeft(sample, bitsPerSample).ToInt64() +
              GetTopBackCenter(sample, bitsPerSample).ToInt64() +
              GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 5;

            var sideRight = (GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 5;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(sideLeft.ToByteArray(bitsPerSample / 8))
                .Concat(sideRight.ToByteArray(bitsPerSample / 8))
                .ToArray();
        }
        public virtual byte[] ToFivePointOne(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64()) / 2;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64()) / 2;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 5;

            var backLeft = (GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 5;

            var backRight = (GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 5;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(GetLowFrequency(sample, bitsPerSample))
                .Concat(backLeft.ToByteArray(bitsPerSample / 8))
                .Concat(backRight.ToByteArray(bitsPerSample / 8))
                .ToArray();
        }
        public virtual byte[] ToSevenPointZero(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64()) / 2;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64()) / 2;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 5;

            var backLeft = (GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 4;

            var backRight = (GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 4;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(backLeft.ToByteArray(bitsPerSample / 8))
                .Concat(backRight.ToByteArray(bitsPerSample / 8))
                .Concat(GetSideLeft(sample, bitsPerSample))
                .Concat(GetSideRight(sample, bitsPerSample))
                .ToArray();
        }
        public virtual byte[] ToSevenPointOne(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64()) / 2;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64()) / 2;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 3;

            var backLeft = (GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 5;

            var backRight = (GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 5;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(GetLowFrequency(sample, bitsPerSample))
                .Concat(backLeft.ToByteArray(bitsPerSample / 8))
                .Concat(backRight.ToByteArray(bitsPerSample / 8))
                .Concat(GetFrontLeftOfCenter(sample, bitsPerSample))
                .Concat(GetFrontRightOfCenter(sample, bitsPerSample))
                .ToArray();
        }
     
        public virtual byte[] ToFivePointOneSurround(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64()) / 2;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64()) / 2;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64()) / 5;

            var sideLeft = (GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideLeft(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 5;

            var sideRight = (GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetSideRight(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 5;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(GetLowFrequency(sample, bitsPerSample))
                .Concat(sideLeft.ToByteArray(bitsPerSample / 8))
                .Concat(sideRight.ToByteArray(bitsPerSample / 8))
                .ToArray();
        }
        public virtual byte[] ToSevenPointOneSurround(byte[] sample, ushort bitsPerSample)
        {
            var frontLeft = (GetFrontLeft(sample, bitsPerSample).ToInt64() +
               GetTopFrontLeft(sample, bitsPerSample).ToInt64()) / 2;

            var frontRight = (GetFrontRight(sample, bitsPerSample).ToInt64() +
               GetTopFrontRight(sample, bitsPerSample).ToInt64()) / 2;

            var frontCenter = (GetFrontCenter(sample, bitsPerSample).ToInt64() +
               GetTopCenter(sample, bitsPerSample).ToInt64() +
               GetTopFrontCenter(sample, bitsPerSample).ToInt64() +
               GetFrontLeftOfCenter(sample, bitsPerSample).ToInt64() +
               GetFrontRightOfCenter(sample, bitsPerSample).ToInt64()) / 5;

            var backLeft = (GetBackLeft(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackLeft(sample, bitsPerSample).ToInt64()) / 4;

            var backRight = (GetBackRight(sample, bitsPerSample).ToInt64() +
               GetBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackCenter(sample, bitsPerSample).ToInt64() +
               GetTopBackRight(sample, bitsPerSample).ToInt64()) / 4;

            return frontLeft.ToByteArray(bitsPerSample / 8)
                .Concat(frontRight.ToByteArray(bitsPerSample / 8))
                .Concat(frontCenter.ToByteArray(bitsPerSample / 8))
                .Concat(GetLowFrequency(sample, bitsPerSample))
                .Concat(backLeft.ToByteArray(bitsPerSample / 8))
                .Concat(backRight.ToByteArray(bitsPerSample / 8))
                .Concat(GetSideLeft(sample, bitsPerSample))
                .Concat(GetSideRight(sample, bitsPerSample))
                .ToArray();
        }

    }
}
