using System;

namespace NtFreX.Audio.Wasapi.Interop
{
    /// <summary>
    /// The _AUDCLNT_BUFFERFLAGS enumeration defines flags that indicate the status of an audio endpoint buffer.
    /// https://docs.microsoft.com/en-us/windows/win32/api/audioclient/ne-audioclient-_audclnt_bufferflags
    /// </summary>
    [Flags]
    internal enum AudioClientBufferFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// The data in the packet is not correlated with the previous packet's device position; this is possibly due to a stream state transition or timing glitch.
        /// </summary>
        DataDiscontinuity = 0x1,

        /// <summary>
        /// Treat all of the data in the packet as silence and ignore the actual data values. 
        /// For more information about the use of this flag, see Rendering a Stream and Capturing a Stream.
        /// </summary>
        Silent = 0x2,

        /// <summary>
        /// The time at which the device's stream position was recorded is uncertain. 
        /// Thus, the client might be unable to accurately set the time stamp for the current data packet.
        /// </summary>
        TimestampError = 0x4
    }
}
