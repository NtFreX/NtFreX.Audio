using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    /// <summary>
    /// The IAudioClient interface enables a client to create and initialize an audio stream between an audio application and the audio engine (for a shared-mode stream) 
    /// or the hardware buffer of an audio endpoint device (for an exclusive-mode stream).
    /// https://docs.microsoft.com/en-us/windows/win32/api/audioclient/nn-audioclient-iaudioclient
    /// </summary>
    [ComImport]
    [Guid(ClsId.IAudioClient)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioClient
    {
        /// <summary>
        /// The Initialize method initializes the audio stream.
        /// </summary>
        /// <param name="shareMode">
        /// The sharing mode for the connection. Through this parameter, the client tells the audio engine whether it wants to share the 
        /// audio endpoint device with other clients. The client should set this parameter to one of the following AUDCLNT_SHAREMODE enumeration values:
        /// AUDCLNT_SHAREMODE_EXCLUSIVE
        /// AUDCLNT_SHAREMODE_SHARED
        /// </param>
        /// <param name="streamFlags">
        /// Flags to control creation of the stream. The client should set this parameter to 0 or to the bitwise OR of one or more of the 
        /// AUDCLNT_STREAMFLAGS_XXX Constants or the AUDCLNT_SESSIONFLAGS_XXX Constants.
        /// </param>
        /// <param name="bufferDuration">
        /// The buffer capacity as a time value. This parameter is of type REFERENCE_TIME and is expressed in 100-nanosecond units. 
        /// This parameter contains the buffer size that the caller requests for the buffer that the audio application will share with 
        /// the audio engine (in shared mode) or with the endpoint device (in exclusive mode). If the call succeeds, 
        /// the method allocates a buffer that is a least this large. For more information about REFERENCE_TIME, see the Windows SDK documentation. 
        /// For more information about buffering requirements, see Remarks.
        /// </param>
        /// <param name="devicePeriod">
        /// The device period. This parameter can be nonzero only in exclusive mode. In shared mode, always set this parameter to 0. 
        /// In exclusive mode, this parameter specifies the requested scheduling period for successive buffer accesses by the audio endpoint device. 
        /// If the requested device period lies outside the range that is set by the device's minimum period and the system's maximum period, 
        /// then the method clamps the period to that range. If this parameter is 0, the method sets the device period to its default value. 
        /// To obtain the default device period, call the IAudioClient::GetDevicePeriod method. If the AUDCLNT_STREAMFLAGS_EVENTCALLBACK stream flag is 
        /// set and AUDCLNT_SHAREMODE_EXCLUSIVE is set as the ShareMode, then hnsPeriodicity must be nonzero and equal to hnsBufferDuration.
        /// </param>
        /// <param name="format">
        /// Pointer to a format descriptor. This parameter must point to a valid format descriptor of type WAVEFORMATEX (or WAVEFORMATEXTENSIBLE). 
        /// For more information, see Remarks.
        /// </param>
        /// <param name="audioSessionId">
        /// Pointer to a session GUID. This parameter points to a GUID value that identifies the audio session that the stream belongs to. 
        /// If the GUID identifies a session that has been previously opened, the method adds the stream to that session. 
        /// If the GUID does not identify an existing session, the method opens a new session and adds the stream to that session. 
        /// The stream remains a member of the same session for its lifetime. Setting this parameter to NULL is equivalent to passing a pointer to a GUID_NULL value.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                            Return code Description
        /// AUDCLNT_E_ALREADY_INITIALIZED           The IAudioClient object is already initialized.
        /// AUDCLNT_E_WRONG_ENDPOINT_TYPE           The AUDCLNT_STREAMFLAGS_LOOPBACK flag is set but the endpoint device is a capture device, not a rendering device.
        /// AUDCLNT_E_BUFFER_SIZE_NOT_ALIGNED       Note  Applies to Windows 7 and later.
        ///                                         The requested buffer size is not aligned. 
        ///                                         This code can be returned for a render or a capture device if the caller 
        ///                                         specified AUDCLNT_SHAREMODE_EXCLUSIVE and the AUDCLNT_STREAMFLAGS_EVENTCALLBACK flags. 
        ///                                         The caller must call Initialize again with the aligned buffer size.For more information, see Remarks.
        /// AUDCLNT_E_BUFFER_SIZE_ERROR             Note  Applies to Windows 7 and later.
        ///                                         Indicates that the buffer duration value requested by an exclusive-mode client is out of range. 
        ///                                         The requested duration value for pull mode must not be greater than 5000 milliseconds; 
        ///                                         for push mode the duration value must not be greater than 2 seconds.
        /// AUDCLNT_E_CPUUSAGE_EXCEEDED             Indicates that the process-pass duration exceeded the maximum CPU usage.
        ///                                         The audio engine keeps track of CPU usage by maintaining the number of times the process-pass 
        ///                                         duration exceeds the maximum CPU usage.The maximum CPU usage is calculated as a percent of the engine's periodicity. 
        ///                                         The percentage value is the system's CPU throttle value(within the range of 10% and 90%). 
        ///                                         If this value is not found, then the default value of 40% is used to calculate the maximum CPU usage.
        /// AUDCLNT_E_DEVICE_INVALIDATED            The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources 
        ///                                         have been reconfigured, disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_DEVICE_IN_USE                 The endpoint device is already in use. Either the device is being used in exclusive mode, or the device is 
        ///                                         being used in shared mode and the caller asked to use the device in exclusive mode.
        /// AUDCLNT_E_ENDPOINT_CREATE_FAILED        The method failed to create the audio endpoint for the render or the capture device. 
        ///                                         This can occur if the audio endpoint device has been unplugged, or the audio hardware 
        ///                                         or associated hardware resources have been reconfigured, disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_INVALID_DEVICE_PERIOD         Note  Applies to Windows 7 and later.
        ///                                         Indicates that the device period requested by an exclusive-mode client is greater than 5000 milliseconds.
        /// AUDCLNT_E_UNSUPPORTED_FORMAT            The audio engine (shared mode) or audio endpoint device (exclusive mode) does not support the specified format.
        /// AUDCLNT_E_EXCLUSIVE_MODE_NOT_ALLOWED    The caller is requesting exclusive-mode use of the endpoint device, but the user has disabled exclusive-mode 
        ///                                         use of the device.
        /// AUDCLNT_E_BUFDURATION_PERIOD_NOT_EQUAL  The AUDCLNT_STREAMFLAGS_EVENTCALLBACK flag is set but parameters hnsBufferDuration and hnsPeriodicity are not equal.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING           The Windows audio service is not running.
        /// E_POINTER                               Parameter pFormat is NULL.
        /// E_INVALIDARG                            Parameter pFormat points to an invalid format description; or the AUDCLNT_STREAMFLAGS_LOOPBACK flag is 
        ///                                         set but ShareMode is not equal to AUDCLNT_SHAREMODE_SHARED; 
        ///                                         or the AUDCLNT_STREAMFLAGS_CROSSPROCESS flag is set but ShareMode is equal to AUDCLNT_SHAREMODE_EXCLUSIVE.
        ///                                         A prior call to SetClientProperties was made with an invalid category for audio/render streams.
        /// E_OUTOFMEMORY                           Out of memory.
        /// </returns>
        [PreserveSig]
        HResult Initialize(
            [In][MarshalAs(UnmanagedType.I4)] AudioClientShareMode shareMode,
            [In][MarshalAs(UnmanagedType.U4)] AudioClientStreamFlags streamFlags,
            [In][MarshalAs(UnmanagedType.U8)] ulong bufferDuration,
            [In][MarshalAs(UnmanagedType.U8)] ulong devicePeriod,
            [In][MarshalAs(UnmanagedType.SysInt)] IntPtr format,
            [In, Optional][MarshalAs(UnmanagedType.LPStruct)] Guid audioSessionId);

        /// <summary>
        /// The GetBufferSize method retrieves the size (maximum capacity) of the endpoint buffer.
        /// </summary>
        /// <param name="size">Pointer to a UINT32 variable into which the method writes the number of audio frames that the buffer can hold.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                    Return code Description
        /// AUDCLNT_E_NOT_INITIALIZED       The audio stream has not been successfully initialized.
        /// AUDCLNT_E_DEVICE_INVALIDATED    The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, 
        ///                                 disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING   The Windows audio service is not running.
        /// E_POINTER                       Parameter pNumBufferFrames is NULL.
        /// </returns>
        [PreserveSig]
        HResult GetBufferSize([Out][MarshalAs(UnmanagedType.U4)] out uint size);

        /// <summary>
        /// The GetStreamLatency method retrieves the maximum latency for the current stream and can be called any time after the stream has been initialized.
        /// </summary>
        /// <param name="latency">
        /// Pointer to a REFERENCE_TIME variable into which the method writes a time value representing the latency. 
        /// The time is expressed in 100-nanosecond units. 
        /// For more information about REFERENCE_TIME, see the Windows SDK documentation.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                    Return code Description
        /// AUDCLNT_E_NOT_INITIALIZED       The audio stream has not been successfully initialized.
        /// AUDCLNT_E_DEVICE_INVALIDATED    The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, 
        ///                                 disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING   The Windows audio service is not running.
        /// E_POINTER                       Parameter phnsLatency is NULL.
        /// </returns>
        [PreserveSig]
        HResult GetStreamLatency([Out][MarshalAs(UnmanagedType.U8)] out ulong latency);

        /// <summary>
        /// The GetCurrentPadding method retrieves the number of frames of padding in the endpoint buffer.
        /// </summary>
        /// <param name="frameCount">Pointer to a UINT32 variable into which the method writes the frame count (the number of audio frames of padding in the buffer).</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                    Return code Description
        /// AUDCLNT_E_NOT_INITIALIZED       The audio stream has not been successfully initialized.
        /// AUDCLNT_E_DEVICE_INVALIDATED    The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, 
        ///                                 disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING   The Windows audio service is not running.
        /// E_POINTER                       Parameter pNumPaddingFrames is NULL.
        /// </returns>
        [PreserveSig]
        HResult GetCurrentPadding([Out][MarshalAs(UnmanagedType.U4)] out uint frameCount);

        /// <summary>
        /// The IsFormatSupported method indicates whether the audio endpoint device supports a particular stream format.
        /// </summary>
        /// <param name="shareMode">
        /// The sharing mode for the stream format. Through this parameter, the client indicates whether it wants to use the specified format in exclusive mode or shared mode.
        /// The client should set this parameter to one of the following AUDCLNT_SHAREMODE enumeration values:
        /// AUDCLNT_SHAREMODE_EXCLUSIVE
        /// AUDCLNT_SHAREMODE_SHARED
        /// </param>
        /// <param name="format">
        /// Pointer to the specified stream format. This parameter points to a caller-allocated format descriptor of type WAVEFORMATEX or WAVEFORMATEXTENSIBLE. 
        /// The client writes a format description to this structure before calling this method. 
        /// For information about WAVEFORMATEX and WAVEFORMATEXTENSIBLE, see the Windows DDK documentation.
        /// </param>
        /// <param name="closestMatch">
        /// Pointer to a pointer variable into which the method writes the address of a WAVEFORMATEX or WAVEFORMATEXTENSIBLE structure. 
        /// This structure specifies the supported format that is closest to the format that the client specified through the pFormat parameter. 
        /// For shared mode (that is, if the ShareMode parameter is AUDCLNT_SHAREMODE_SHARED), set ppClosestMatch to point to a valid, non-NULL pointer variable. 
        /// For exclusive mode, set ppClosestMatch to NULL. The method allocates the storage for the structure. The caller is responsible for freeing the storage,
        /// when it is no longer needed, by calling the CoTaskMemFree function. If the IsFormatSupported call fails and ppClosestMatch is non-NULL, 
        /// the method sets *ppClosestMatch to NULL. For information about CoTaskMemFree, see the Windows SDK documentation.
        /// </param>
        /// <returns>
        /// Return code                     Description
        /// S_OK                            Succeeded and the audio endpoint device supports the specified stream format.
        /// S_FALSE                         Succeeded with a closest match to the specified format.
        /// AUDCLNT_E_UNSUPPORTED_FORMAT    Succeeded but the specified format is not supported in exclusive mode.
        /// 
        /// If the operation fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                    Return code Description
        /// E_POINTER                       Parameter pFormat is NULL, or ppClosestMatch is NULL and ShareMode is AUDCLNT_SHAREMODE_SHARED.
        /// E_INVALIDARG                    Parameter ShareMode is a value other than AUDCLNT_SHAREMODE_SHARED or AUDCLNT_SHAREMODE_EXCLUSIVE.
        /// AUDCLNT_E_DEVICE_INVALIDATED    The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, 
        ///                                 disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING   The Windows audio service is not running.        
        /// </returns>
        [PreserveSig]
        HResult IsFormatSupported(
            [In][MarshalAs(UnmanagedType.I4)] AudioClientShareMode shareMode,
            [In][MarshalAs(UnmanagedType.SysInt)] IntPtr format,
            [Out] out IntPtr closestMatch);

        /// <summary>
        /// The GetMixFormat method retrieves the stream format that the audio engine uses for its internal processing of shared-mode streams.
        /// </summary>
        /// <param name="format">
        /// Pointer to a pointer variable into which the method writes the address of the mix format. 
        /// This parameter must be a valid, non-NULL pointer to a pointer variable. 
        /// The method writes the address of a WAVEFORMATEX (or WAVEFORMATEXTENSIBLE) structure to this variable. 
        /// The method allocates the storage for the structure. The caller is responsible for freeing the storage, when it is no longer needed, 
        /// by calling the CoTaskMemFree function. If the GetMixFormat call fails, *ppDeviceFormat is NULL. 
        /// For information about WAVEFORMATEX, WAVEFORMATEXTENSIBLE, and CoTaskMemFree, see the Windows SDK documentation.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                    Return code Description
        /// AUDCLNT_E_DEVICE_INVALIDATED    The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, 
        ///                                 disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING   The Windows audio service is not running.
        /// E_POINTER                       Parameter ppDeviceFormat is NULL.
        /// E_OUTOFMEMORY                   Out of memory.
        /// </returns>
        [PreserveSig]
        HResult GetMixFormat([Out][MarshalAs(UnmanagedType.SysInt)] out IntPtr format);

        /// <summary>
        /// The GetDevicePeriod method retrieves the length of the periodic interval separating successive processing passes by the audio engine 
        /// on the data in the endpoint buffer.
        /// </summary>
        /// <param name="defaultDevicePeriod">
        /// Pointer to a REFERENCE_TIME variable into which the method writes a time value specifying the default interval between periodic processing passes 
        /// by the audio engine. The time is expressed in 100-nanosecond units. For information about REFERENCE_TIME, see the Windows SDK documentation.
        /// </param>
        /// <param name="minimumDevicePeriod">
        /// Pointer to a REFERENCE_TIME variable into which the method writes a time value specifying the minimum interval between periodic processing passes
        /// by the audio endpoint device. The time is expressed in 100-nanosecond units.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                    Return code Description
        /// AUDCLNT_E_DEVICE_INVALIDATED    The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, 
        ///                                 disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING   The Windows audio service is not running.
        /// E_POINTER                       Parameters phnsDefaultDevicePeriod and phnsMinimumDevicePeriod are both NULL.
        /// </returns>
        [PreserveSig]
        HResult GetDevicePeriod(
            [Out][MarshalAs(UnmanagedType.U8)] out ulong defaultDevicePeriod,
            [Out][MarshalAs(UnmanagedType.U8)] out ulong minimumDevicePeriod);

        /// <summary>
        /// The Start method starts the audio stream.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                                Return code Description
        /// AUDCLNT_E_NOT_INITIALIZED                   The audio stream has not been successfully initialized.
        /// AUDCLNT_E_NOT_STOPPEDq                      The audio stream was not stopped at the time of the Start call.
        /// AUDCLNT_E_EVENTHANDLE_NOT_SET               The audio stream is configured to use event-driven buffering, but the caller has not called 
        ///                                             IAudioClient::SetEventHandle to set the event handle on the stream.
        /// AUDCLNT_E_DEVICE_INVALIDATED                The audio endpoint device has been unplugged, or the audio hardware or associated hardware 
        ///                                             resources have been reconfigured, disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING               The Windows audio service is not running.
        /// </returns>
        [PreserveSig]
        HResult Start();

        /// <summary>
        /// The Stop method stops the audio stream.
        /// </summary>
        /// <returns>
        /// If the method succeeds and stops the stream, it returns S_OK. If the method succeeds and the stream was already stopped, the method returns S_FALSE. 
        /// If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                        Return code Description
        /// AUDCLNT_E_NOT_INITIALIZED           The client has not been successfully initialized.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING       The Windows audio service is not running.
        /// </returns>
        [PreserveSig]
        HResult Stop();

        /// <summary>
        /// The Reset method resets the audio stream.
        /// </summary>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If the method succeeds and the stream was already reset, the method returns S_FALSE. 
        /// If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                        Return code Description
        /// AUDCLNT_E_NOT_INITIALIZED           The audio stream has not been successfully initialized.
        /// AUDCLNT_E_NOT_STOPPED               The audio stream was not stopped at the time the call was made.
        /// AUDCLNT_E_BUFFER_OPERATION_PENDING  The client is currently writing to or reading from the buffer.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING       The Windows audio service is not running.
        /// </returns>
        [PreserveSig]
        HResult Reset();

        /// <summary>
        /// The SetEventHandle method sets the event handle that the system signals when an audio buffer is ready to be processed by the client.
        /// </summary>
        /// <param name="handle">The event handle.</param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                            Return code Description
        /// E_INVALIDARG                            Parameter eventHandle is NULL or an invalid handle.
        /// AUDCLNT_E_EVENTHANDLE_NOT_EXPECTED      The audio stream was not initialized for event-driven buffering.
        /// AUDCLNT_E_NOT_INITIALIZED               The audio stream has not been successfully initialized.
        /// AUDCLNT_E_DEVICE_INVALIDATED            The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources 
        ///                                         have been reconfigured, disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING           The Windows audio service is not running.
        /// </returns>
        [PreserveSig]
        HResult SetEventHandle([In][MarshalAs(UnmanagedType.SysInt)] IntPtr handle);

        /// <summary>
        /// The GetService method accesses additional services from the audio client object.
        /// </summary>
        /// <param name="interfaceId">
        /// The interface ID for the requested service. The client should set this parameter to one of the following REFIID values:
        /// IID_IAudioCaptureClient
        /// IID_IAudioClock
        /// IID_IAudioRenderClient
        /// IID_IAudioSessionControl
        /// IID_IAudioStreamVolume
        /// IID_IChannelAudioVolume
        /// IID_IMFTrustedOutput
        /// IID_ISimpleAudioVolume
        /// </param>
        /// <param name="instancePtr">
        /// Pointer to a pointer variable into which the method writes the address of an instance of the requested interface. 
        /// Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, 
        /// when it is no longer needed, by calling the interface's Release method. If the GetService call fails, *ppv is NULL.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE                    Return code Description
        /// E_POINTER                       Parameter ppv is NULL.
        /// E_NOINTERFACE                   The requested interface is not available.
        /// AUDCLNT_E_NOT_INITIALIZED       The audio stream has not been initialized.
        /// AUDCLNT_E_WRONG_ENDPOINT_TYPE   The caller tried to access an IAudioCaptureClient interface on a rendering endpoint, or an IAudioRenderClient interface 
        ///                                 on a capture endpoint.
        /// AUDCLNT_E_DEVICE_INVALIDATED    The audio endpoint device has been unplugged, or the audio hardware or associated hardware resources have been reconfigured, 
        ///                                 disabled, removed, or otherwise made unavailable for use.
        /// AUDCLNT_E_SERVICE_NOT_RUNNING   The Windows audio service is not running.
        /// </returns>
        [PreserveSig]
        HResult GetService(
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId,                           
            [Out][MarshalAs(UnmanagedType.IUnknown)] out object instancePtr);
    }
}
