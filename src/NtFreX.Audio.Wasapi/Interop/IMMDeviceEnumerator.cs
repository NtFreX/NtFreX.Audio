using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    /// <summary>
    /// The IMMDeviceEnumerator interface provides methods for enumerating multimedia device resources. 
    /// In the current implementation of the MMDevice API, the only device resources that this interface can enumerate are audio endpoint devices. 
    /// A client obtains a reference to an IMMDeviceEnumerator interface by calling the CoCreateInstance function (see MMDevice API).
    /// The device resources enumerated by the methods in the IMMDeviceEnumerator interface are represented as collections of objects with IMMDevice interfaces.
    /// A collection has an IMMDeviceCollection interface. The IMMDeviceEnumerator::EnumAudioEndpoints method creates a device collection.
    /// To obtain a pointer to the IMMDevice interface of an item in a device collection, the client calls the IMMDeviceCollection::Item method.
    /// defined in mmdeviceapi.h
    /// https://docs.microsoft.com/en-us/windows/win32/api/mmdeviceapi/nn-mmdeviceapi-immdeviceenumerator
    /// https://github.com/jamesjharper/nFundamental/blob/master/src/nFundamental.Interface.Wasapi/Interop/IMMDeviceEnumerator.cs
    /// </summary>
    [ComImport]
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {

        /// <summary>
        /// The EnumAudioEndpoints method generates a collection of audio endpoint devices that meet the specified criteria.
        /// </summary>
        /// <param name="dataFlow">
        /// The data-flow direction for the endpoint devices in the collection. The caller should set this parameter to one of the following EDataFlow enumeration values:
        /// eRender
        /// eCapture
        /// eAll
        /// If the caller specifies eAll, the method includes both rendering and capture endpoints in the collection.
        /// </param>
        /// <param name="deviceState">
        /// The state or states of the endpoints that are to be included in the collection. The caller should set this parameter to the bitwise OR of one or more of the following DeviceState constants:
        /// ACTIVE
        /// DISABLED
        /// NOTPRESENT
        /// UNPLUGGED
        /// For example, if the caller sets the deviceState parameter to DEVICE_STATE_ACTIVE | DEVICE_STATE_UNPLUGGED, 
        /// the method includes endpoints that are either active or unplugged from their jacks, 
        /// but excludes endpoints that are on audio adapters that have been disabled or are not present.
        /// To include all endpoints, regardless of state, set deviceState = DEVICE_STATEMASK_ALL.
        /// </param>
        /// <param name="devices">
        /// Pointer to a pointer variable into which the method writes the address of the IMMDeviceCollection interface of the device-collection object. 
        /// Through this method, the caller obtains a counted reference to the interface. 
        /// The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. 
        /// If the EnumAudioEndpoints call fails, *devices is NULL.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE    Return code Description
        /// E_POINTER       Parameter devices is NULL.
        /// E_INVALIDARG    Parameter dataFlow or deviceState is out of range.
        /// E_OUTOFMEMORY   Out of memory.
        /// </returns>
        [PreserveSig] HResult EnumAudioEndpoints(
            [In] DataFlow dataFlow,
            [In] DeviceState deviceState,
            [Out] out IMMDeviceCollection devices);
        
        
        
    }
}
