namespace NtFreX.Audio.PulseAudio.Interop
{
    /// <summary>
    /// A sample format and attribute specification
    /// https://freedesktop.org/software/pulseaudio/doxygen/structpa__sample__spec.html
    /// </summary>
    internal struct pa_sample_spec
    {
        /// <summary>
        /// The sample format.
        /// </summary>
        public pa_sample_format format;

        /// <summary>
        /// The sample rate.
        /// (e.g. 44100)
        /// </summary>
        public uint rate;

        /// <summary>
        /// Audio channels.
        /// (1 for mono, 2 for stereo, ...)
        /// </summary>
        public byte channels;
    }
}
