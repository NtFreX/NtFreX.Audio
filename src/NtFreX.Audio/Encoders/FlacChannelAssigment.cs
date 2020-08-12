namespace NtFreX.Audio.Encoders
{
    internal enum FlacChannelAssigment
    {
        /// <summary>
        /// independent channels
        /// </summary>
        FLAC__CHANNEL_ASSIGNMENT_INDEPENDENT = 0,

        /// <summary>
        /// left+side stereo
        /// </summary>
        FLAC__CHANNEL_ASSIGNMENT_LEFT_SIDE = 1,

        /// <summary>
        /// right+side stereo
        /// </summary>
        FLAC__CHANNEL_ASSIGNMENT_RIGHT_SIDE = 2,

        /// <summary>
        /// mid+side stereo
        /// </summary>
        FLAC__CHANNEL_ASSIGNMENT_MID_SIDE = 3
    }
}
