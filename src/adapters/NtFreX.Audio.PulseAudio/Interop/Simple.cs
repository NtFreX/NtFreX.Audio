using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.PulseAudio.Interop
{
    /// <summary>
    /// A simple but limited synchronous playback and recording API.
    /// https://freedesktop.org/software/pulseaudio/doxygen/simple_8h.html#details
    /// https://github.com/terrafx/terrafx.interop.pulseaudio/blob/5b289256882fd15cfc376ada048b2d179196cddf/sources/Interop/PulseAudio/simple/Pulse.cs
    /// </summary>
    internal static class Simple
    {
        const string Library = "libpulse.so.0";

        /// <summary>
        /// Create a new connection to the server.
        /// </summary>
        /// <param name="server">Server name, or NULL for default</param>
        /// <param name="name">A descriptive name for this client (application name, ...)</param>
        /// <param name="dir">Open this stream for recording or playback?</param>
        /// <param name="dev">Sink (resp. source) name, or NULL for default</param>
        /// <param name="stream_name">A descriptive name for this stream (application name, song title, ...)</param>
        /// <param name="ss">The sample type to use</param>
        /// <param name="map">The channel map to use, or NULL for default</param>
        /// <param name="attr">Buffering attributes, or NULL for default</param>
        /// <param name="error">A pointer where the error code is stored when the routine returns NULL. It is OK to pass NULL here.</param>
        /// <returns></returns>
        [DllImport(Library, EntryPoint = "pa_simple_new", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern pa_simple pa_simple_new(string server, string name, pa_stream_direction dir, string dev, string stream_name, pa_sample_spec ss, pa_channel_map map, pa_buffer_attr attr, ref int error);

        [DllImport(Library, EntryPoint = "pa_simple_free", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern void pa_simple_free(pa_simple s);

        [DllImport(Library, EntryPoint = "pa_simple_write", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int pa_simple_write(pa_simple s, IntPtr data, uint bytes, ref int error);

        [DllImport(Library, EntryPoint = "pa_simple_drain", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int pa_simple_drain(pa_simple s, ref int error);

        [DllImport(Library, EntryPoint = "pa_simple_read", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int pa_simple_read(pa_simple s, IntPtr data, uint bytes, ref int error);

        [DllImport(Library, EntryPoint = "pa_simple_get_latency", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern ulong pa_simple_get_latency(pa_simple s, ref int error);

        [DllImport(Library, EntryPoint = "pa_simple_flush", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, CharSet = CharSet.Ansi)]
        public static extern int pa_simple_flush(pa_simple s, ref int error);
    }
}
