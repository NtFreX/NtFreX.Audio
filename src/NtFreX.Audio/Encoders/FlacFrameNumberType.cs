namespace NtFreX.Audio.Encoders
{
    internal enum FlacFrameNumberType 
    {
        /// <summary>
        /// number contains the frame number
        /// </summary>
        FLAC__FRAME_NUMBER_TYPE_FRAME_NUMBER,

        /// <summary>
        /// number contains the sample number of first sample in frame
        /// </summary>
        FLAC__FRAME_NUMBER_TYPE_SAMPLE_NUMBER
    }
}
