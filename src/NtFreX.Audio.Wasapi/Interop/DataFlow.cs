using System;

namespace NtFreX.Audio.Wasapi.Interop
{
    /// <summary>
    /// The EDataFlow enumeration defines constants that indicate the direction in which audio data flows between an audio endpoint device and an application.
    /// __MIDL___MIDL_itf_mmdeviceapi_0000_0000_0001
    /// EDataFlow
    /// defined in mmdeviceapi.h
    /// https://docs.microsoft.com/en-us/windows/win32/api/mmdeviceapi/ne-mmdeviceapi-edataflow
    /// </summary>
    [Flags]
    internal enum DataFlow : uint
    {
        /// <summary>
        /// Audio rendering stream. Audio data flows from the application to the audio endpoint device, which renders the stream.
        /// </summary>
        Render = 0,
        /// <summary>
        /// Audio capture stream. Audio data flows from the audio endpoint device that captures the stream, to the application.
        /// </summary>
        Capture = 1,
        /// <summary>
        /// Audio rendering or capture stream. Audio data can flow either from the application to the audio endpoint device, or from the audio endpoint device to the application.
        /// </summary>
        All = 2,
    }
}
