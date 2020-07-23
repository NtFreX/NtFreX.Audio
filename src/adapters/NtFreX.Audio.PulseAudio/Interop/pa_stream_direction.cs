namespace NtFreX.Audio.PulseAudio.Interop
{
    /// <summary>
    /// The direction of a pa_stream object.
    /// </summary>
    internal enum pa_stream_direction
    {
        /// <summary>
        /// Invalid direction.
        /// </summary>
        PA_STREAM_NODIRECTION,
        /// <summary>
        /// Playback stream.
        /// </summary>
        PA_STREAM_PLAYBACK,
        /// <summary>
        /// Record stream.
        /// </summary>
        PA_STREAM_RECORD,
        /// <summary>
        /// Sample upload stream
        /// </summary>
        PA_STREAM_UPLOAD
    }
}
