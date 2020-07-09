namespace NtFreX.Audio.PulseAudio.Interop
{
    /// <summary>
    /// Sample format.
    /// </summary>
    internal enum pa_sample_format
    {
        /// <summary>
        /// Unsigned 8 Bit PCM.
        /// </summary>
        PA_SAMPLE_U8,
        /// <summary>
        /// 8 Bit a-Law
        /// </summary>
        PA_SAMPLE_ALAW,
        /// <summary>
        /// 8 Bit mu-Law
        /// </summary>
        PA_SAMPLE_ULAW,
        /// <summary>
        /// Signed 16 Bit PCM, little endian (PC)
        /// </summary>
        PA_SAMPLE_S16LE,
        /// <summary>
        /// Signed 16 Bit PCM, big endian.
        /// </summary>
        PA_SAMPLE_S16BE,
        /// <summary>
        /// 32 Bit IEEE floating point, little endian (PC), range -1.0 to 1.0
        /// </summary>
        PA_SAMPLE_FLOAT32LE,
        /// <summary>
        /// 32 Bit IEEE floating point, big endian, range -1.0 to 1.0
        /// </summary>
        PA_SAMPLE_FLOAT32BE,
        /// <summary>
        /// Signed 32 Bit PCM, little endian (PC)
        /// </summary>
        PA_SAMPLE_S32LE,
        /// <summary>
        /// Signed 32 Bit PCM, big endian.
        /// </summary>
        PA_SAMPLE_S32BE,
        /// <summary>
        /// Signed 24 Bit PCM packed, little endian (PC).
        /// Since 0.9.15
        /// </summary>
        PA_SAMPLE_S24LE,
        /// <summary>
        /// Signed 24 Bit PCM packed, big endian
        /// Since 0.9.15
        /// </summary>
        PA_SAMPLE_S24BE,
        /// <summary>
        /// Signed 24 Bit PCM in LSB of 32 Bit words, little endian (PC).
        /// Since 0.9.15
        /// </summary>
        PA_SAMPLE_S24_32LE,
        /// <summary>
        /// Signed 24 Bit PCM in LSB of 32 Bit words, big endian.
        /// Since 0.9.15
        /// </summary>
        PA_SAMPLE_S24_32BE,
        /// <summary>
        /// Upper limit of valid sample types.
        /// </summary>
        PA_SAMPLE_MAX,
        /// <summary>
        /// An invalid value.
        /// </summary>
        PA_SAMPLE_INVALID = -1,
    }
}
