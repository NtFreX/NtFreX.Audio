using System;

namespace NtFreX.Audio.Infrastructure
{
    /// <summary>
    /// ksmedia.h
    /// </summary>
    [Flags]
    public enum Speaker: uint
    {
        FrontLeft = 0x1,
        FrontRight = 0x2,
        FrontCenter = 0x4,
        LowFrequency = 0x8,
        BackLeft = 0x10,
        BackRight = 0x20,
        FrontLeftOfCenter = 0x40,
        FrontRightOfCenter = 0x80,
        BackCenter = 0x100,
        SideLeft = 0x200,
        SideRight = 0x400,
        TopCenter = 0x800,
        TopFrontLeft = 0x1000,
        TopFrontCenter = 0x2000,
        TopFrontRight = 0x4000,
        TopBackLeft = 0x8000,
        TopBackCenter = 0x10000,
        TopBackRight = 0x20000,

        /// <summary>
        /// Used to specify that any possible permutation of speaker configurations
        /// </summary>
        SpeakerAll = 0x80000000,

        Mono = FrontCenter,
        OnePointOne = FrontCenter | LowFrequency,
        Stereo = FrontLeft | FrontRight,
        TwoPointOne = FrontLeft | FrontRight | LowFrequency,
        ThreePointZero = FrontLeft | FrontRight | FrontCenter,
        ThreePointOne = FrontLeft | FrontRight | FrontCenter | LowFrequency,
        Quad = FrontLeft | FrontRight | BackLeft | BackRight,
        Surround = FrontLeft | FrontRight | FrontCenter | BackCenter,
        FivePointZero = FrontLeft | FrontRight | FrontCenter | SideLeft | SideRight,
        FivePointOne = FrontLeft | FrontRight | FrontCenter | LowFrequency | BackLeft | BackRight,
        SevenPointZero = FrontLeft | FrontRight | FrontCenter | BackLeft | BackRight | SideLeft | SideRight,
        SevenPointOne = FrontLeft | FrontRight | FrontCenter | LowFrequency | BackLeft | BackRight | FrontLeftOfCenter | FrontRightOfCenter,

        FivePointOneSurround = FrontLeft | FrontRight | FrontCenter | LowFrequency | SideLeft | SideRight,
        SevenPointOneSurround = FrontLeft | FrontRight | FrontCenter | LowFrequency | BackLeft | BackRight | SideLeft | SideRight
    }
}
