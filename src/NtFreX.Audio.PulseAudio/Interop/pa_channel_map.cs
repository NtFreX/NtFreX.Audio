using System.Runtime.InteropServices;

namespace NtFreX.Audio.PulseAudio.Interop
{

    /// <summary>
    /// A channel map which can be used to attach labels to specific channels of a stream.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class pa_channel_map
    {
        /// <summary>
        /// Maximum number of allowed channels.
        /// </summary>
        private const int PA_CHANNELS_MAX = 32;

        /// <summary>
        /// Number of channels mapped.
        /// </summary>
        public byte channels;

        /// <summary>
        /// Channel labels.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PA_CHANNELS_MAX)]
        public pa_channel_position[] map;
    }
}
