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
        [PreserveSig] HResult EnumAudioEndpoints([In] DataFlow dataFlow, [In] DeviceState deviceState, [Out] out IMMDeviceCollection devices);

        /// <summary>
        /// The GetDefaultAudioEndpoint method retrieves the default audio endpoint for the specified data-flow direction and role.
        /// </summary>
        /// <param name="dataFlow">
        /// The data-flow direction for the endpoint device. The caller should set this parameter to one of the following two DataFlow enumeration values:
        /// Render
        /// Capture
        /// The data-flow direction for a rendering device is eRender.The data-flow direction for a capture device is eCapture.
        /// </param>
        /// <param name="role">
        /// The role of the endpoint device. The caller should set this parameter to one of the following Role enumeration values:
        /// Console
        /// Multimedia
        /// Communications
        /// </param>
        /// <param name="ppEndpoint">
        /// Pointer to a pointer variable into which the method writes the address of the IMMDevice interface of the endpoint object for the default audio endpoint device. 
        /// Through this method, the caller obtains a counted reference to the interface. 
        /// The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. 
        /// If the GetDefaultAudioEndpoint call fails, ppDevice is NULL.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, possible return codes include, but are not limited to, the values shown in the following table.
        /// RETURN VALUE    Return code Description
        /// E_POINTER       Parameter ppDevice is NULL.
        /// E_INVALIDARG    Parameter dataFlow or role is out of range.
        /// E_NOTFOUND      No device is available.
        /// E_OUTOFMEMORY   Out of memory.
        /// </returns>
        [PreserveSig] HResult GetDefaultAudioEndpoint([In] DataFlow dataFlow, [In] Role role, [Out] out IMMDevice ppEndpoint);

        [PreserveSig]
        HResult GetDevice([In, MarshalAs(UnmanagedType.LPWStr)] string id, [Out] out IMMDevice device);

        [PreserveSig]
        HResult RegisterEndpointNotificationCallback([In] IMMNotificationClient notificationClient);

        [PreserveSig]
        HResult UnregisterEndpointNotificationCallback([In] IMMNotificationClient notificationClient);
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct PropVariant
    {
        public static PropVariant Empty = FromObject(null);


        [FieldOffset(0)] private ushort varType;
        [FieldOffset(2)] private ushort wReserved1;
        [FieldOffset(4)] private ushort wReserved2;
        [FieldOffset(6)] private ushort wReserved3;

        [FieldOffset(8)] private byte bVal;
        [FieldOffset(8)] private sbyte cVal;
        [FieldOffset(8)] private ushort uiVal;
        [FieldOffset(8)] private short iVal;
        [FieldOffset(8)] private UInt32 uintVal;
        [FieldOffset(8)] private Int32 intVal;
        [FieldOffset(8)] private UInt64 ulVal;
        [FieldOffset(8)] private Int64 lVal;
        [FieldOffset(8)] private float fltVal;
        [FieldOffset(8)] private double dblVal;
        [FieldOffset(8)] private short boolVal;
        [FieldOffset(8)] private IntPtr pclsidVal; //this is for GUID ID pointer
        [FieldOffset(8)] private IntPtr pszVal; //this is for ansi string pointer
        [FieldOffset(8)] private IntPtr pwszVal; //this is for Unicode string pointer
        [FieldOffset(8)] private IntPtr punkVal; //this is for punkVal (interface pointer)
        [FieldOffset(8)] private PropArray ca;
        [FieldOffset(8)] private System.Runtime.InteropServices.ComTypes.FILETIME filetime;




        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        public VariantType ValueType => (VariantType)varType;

        /// <summary>
        /// Determines whether [is variant type supported].
        /// </summary>
        /// <returns></returns>
        public bool IsVariantTypeSupported()
        {
            var vt = (VariantType)varType;

            if ((vt & VariantType.VT_VECTOR) != 0)
            {
                switch (vt & (~VariantType.VT_VECTOR))
                {
                    case VariantType.VT_EMPTY:
                        return true;

                    case VariantType.VT_I1:
                        return true;

                    case VariantType.VT_UI1:
                        return true;

                    case VariantType.VT_I2:
                        return true;

                    case VariantType.VT_UI2:
                        return true;

                    case VariantType.VT_I4:
                        return true;

                    case VariantType.VT_UI4:
                        return true;

                    case VariantType.VT_I8:
                        return true;

                    case VariantType.VT_UI8:
                        return true;

                    case VariantType.VT_R4:
                        return true;

                    case VariantType.VT_R8:
                        return true;

                    case VariantType.VT_BOOL:
                        return true;

                    case VariantType.VT_CLSID:
                        return true;

                    case VariantType.VT_LPSTR:
                        return true;

                    case VariantType.VT_LPWSTR:
                        return true;
                }
            }
            else
            {
                switch (vt)
                {
                    case VariantType.VT_EMPTY:
                        return true;

                    case VariantType.VT_I1:
                        return true;

                    case VariantType.VT_UI1:
                        return true;

                    case VariantType.VT_I2:
                        return true;

                    case VariantType.VT_UI2:
                        return true;

                    case VariantType.VT_I4:
                        return true;

                    case VariantType.VT_UI4:
                        return true;

                    case VariantType.VT_I8:
                        return true;

                    case VariantType.VT_UI8:
                        return true;

                    case VariantType.VT_R4:
                        return true;

                    case VariantType.VT_R8:
                        return true;

                    case VariantType.VT_FILETIME:
                        return true;

                    case VariantType.VT_BOOL:
                        return true;

                    case VariantType.VT_CLSID:
                        return true;

                    case VariantType.VT_LPSTR:
                        return true;

                    case VariantType.VT_LPWSTR:
                        return true;
                    case VariantType.VT_BLOB:
                        return true;
                    case VariantType.VT_NULL:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a prop variant from a given object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static PropVariant FromObject(object source)
        {
            var propVariant = new PropVariant();
            propVariant.Init(source);
            return propVariant;
        }

        /// <summary>
        /// To the object.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public object ToObject()
        {
            var vt = (VariantType)varType;

            if ((vt & VariantType.VT_VECTOR) != 0)
            {
                switch (vt & (~VariantType.VT_VECTOR))
                {
                    case VariantType.VT_EMPTY:
                        return null;

                    case VariantType.VT_I1:
                        {
                            var array = new sbyte[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                                array[i] = (sbyte)Marshal.ReadByte(ca.pElems, i);
                            return array;
                        }

                    case VariantType.VT_UI1:
                        {
                            var array = new byte[ca.cElems];
                            Marshal.Copy(ca.pElems, array, 0, (int)ca.cElems);
                            return array;
                        }

                    case VariantType.VT_I2:
                        {
                            var array = new short[ca.cElems];
                            Marshal.Copy(ca.pElems, array, 0, (int)ca.cElems);
                            return array;
                        }

                    case VariantType.VT_UI2:
                        {
                            var array = new ushort[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                                array[i] = (ushort)Marshal.ReadInt16(ca.pElems, i * sizeof(ushort));
                            return array;
                        }

                    case VariantType.VT_I4:
                        {
                            var array = new int[ca.cElems];
                            Marshal.Copy(ca.pElems, array, 0, (int)ca.cElems);
                            return array;
                        }

                    case VariantType.VT_UI4:
                        {
                            var array = new uint[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                                array[i] = (uint)Marshal.ReadInt32(ca.pElems, i * sizeof(uint));
                            return array;
                        }

                    case VariantType.VT_I8:
                        {
                            var array = new Int64[ca.cElems];
                            Marshal.Copy(ca.pElems, array, 0, (int)ca.cElems);
                            return array;
                        }

                    case VariantType.VT_UI8:
                        {
                            var array = new UInt64[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                                array[i] = (UInt64)Marshal.ReadInt64(ca.pElems, i * sizeof(UInt64));
                            return array;
                        }

                    case VariantType.VT_R4:
                        {
                            var array = new float[ca.cElems];
                            Marshal.Copy(ca.pElems, array, 0, (int)ca.cElems);
                            return array;
                        }

                    case VariantType.VT_R8:
                        {
                            var array = new double[ca.cElems];
                            Marshal.Copy(ca.pElems, array, 0, (int)ca.cElems);
                            return array;
                        }

                    case VariantType.VT_BOOL:
                        {
                            var array = new bool[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                                array[i] = (bool)(Marshal.ReadInt16(ca.pElems, i * sizeof(ushort)) != 0);
                            return array;
                        }

                    case VariantType.VT_CLSID:
                        {
                            var array = new Guid[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                            {
                                var guid = new byte[16];
                                Marshal.Copy(ca.pElems, guid, i * 16, 16);
                                array[i] = new Guid(guid);
                            }
                            return array;
                        }

                    case VariantType.VT_LPSTR:
                        {
                            var array = new string[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                            {
                                var ptr = Marshal.ReadIntPtr(ca.pElems, i * IntPtr.Size);
                                array[i] = Marshal.PtrToStringAnsi(ptr);
                            }
                            return array;
                        }

                    case VariantType.VT_LPWSTR:
                        {
                            var array = new string[ca.cElems];
                            for (var i = 0; i < ca.cElems; i++)
                            {
                                var ptr = Marshal.ReadIntPtr(ca.pElems, i * IntPtr.Size);
                                array[i] = Marshal.PtrToStringUni(ptr);
                            }
                            return array;
                        }

                    case VariantType.VT_UNKNOWN:
                    default:
                        break;
                }
            }
            else
            {
                switch (vt)
                {
                    case VariantType.VT_EMPTY:
                        return null;
                    case VariantType.VT_NULL:
                        return null;

                    case VariantType.VT_I1:
                        return cVal;
                    case VariantType.VT_UI1:
                        return bVal;

                    case VariantType.VT_I2:
                        return iVal;

                    case VariantType.VT_UI2:
                        return uiVal;

                    case VariantType.VT_I4:
                        return intVal;

                    case VariantType.VT_UI4:
                        return uintVal;

                    case VariantType.VT_I8:
                        return lVal;

                    case VariantType.VT_UI8:
                        return ulVal;

                    case VariantType.VT_R4:
                        return fltVal;

                    case VariantType.VT_R8:
                        return dblVal;

                    case VariantType.VT_FILETIME:
                        return filetime;

                    case VariantType.VT_BOOL:
                        return (bool)(boolVal != 0);

                    case VariantType.VT_CLSID:
                        var guid = new byte[16];
                        Marshal.Copy(pclsidVal, guid, 0, 16);
                        return new Guid(guid);

                    case VariantType.VT_LPSTR:
                        return Marshal.PtrToStringAnsi(pszVal);

                    case VariantType.VT_LPWSTR:
                        return Marshal.PtrToStringUni(pwszVal);

                    case VariantType.VT_BLOB:
                        {
                            var blob = new byte[ca.cElems];
                            Marshal.Copy(ca.pElems, blob, 0, (int)ca.cElems);
                            return blob;
                        }
                }
            }

            throw new System.NotSupportedException();
        }


        /// <SecurityNote>
        /// Critical -Accesses unmanaged code and structure is overlapping in memory
        /// TreatAsSafe - inputs are verified or safe
        /// </SecurityNote>
        private void Init(object value)
        {
            if (value == null)
            {
                varType = (ushort)VariantType.VT_EMPTY;
            }
            else if (value is Array)
            {
                var type = value.GetType();

                if (type == typeof(sbyte[]))
                {
                    InitVector(value as Array, typeof(sbyte), VariantType.VT_I1);
                }
                else if (type == typeof(byte[]))
                {
                    InitVector(value as Array, typeof(byte), VariantType.VT_UI1);
                }
                else if (value is char[])
                {
                    varType = (ushort)VariantType.VT_LPSTR;
                    pszVal = Marshal.StringToCoTaskMemAnsi(new string(value as char[]));
                }
                else if (value is char[][])
                {
                    var charArray = value as char[][];

                    var strArray = new string[charArray.GetLength(0)];

                    for (var i = 0; i < charArray.Length; i++)
                    {
                        strArray[i] = new string(charArray[i] as char[]);
                    }

                    Init(strArray, true);
                }
                else if (type == typeof(short[]))
                {
                    InitVector(value as Array, typeof(short), VariantType.VT_I2);
                }
                else if (type == typeof(ushort[]))
                {
                    InitVector(value as Array, typeof(ushort), VariantType.VT_UI2);
                }
                else if (type == typeof(int[]))
                {
                    InitVector(value as Array, typeof(int), VariantType.VT_I4);
                }
                else if (type == typeof(uint[]))
                {
                    InitVector(value as Array, typeof(uint), VariantType.VT_UI4);
                }
                else if (type == typeof(Int64[]))
                {
                    InitVector(value as Array, typeof(Int64), VariantType.VT_I8);
                }
                else if (type == typeof(UInt64[]))
                {
                    InitVector(value as Array, typeof(UInt64), VariantType.VT_UI8);
                }
                else if (value is float[])
                {
                    InitVector(value as Array, typeof(float), VariantType.VT_R4);
                }
                else if (value is double[])
                {
                    InitVector(value as Array, typeof(double), VariantType.VT_R8);
                }
                else if (value is Guid[])
                {
                    InitVector(value as Array, typeof(Guid), VariantType.VT_CLSID);
                }
                else if (value is string[])
                {
                    Init(value as string[], false);
                }
                else if (value is bool[])
                {
                    var boolArray = value as bool[];
                    var array = new short[boolArray.Length];

                    for (var i = 0; i < boolArray.Length; i++)
                    {
                        array[i] = (short)(boolArray[i] ? -1 : 0);
                    }

                    InitVector(array, typeof(short), VariantType.VT_BOOL);
                }
                else
                {
                    throw new System.NotSupportedException();
                }
            }
            else
            {
                var type = value.GetType();

                if (value is string)
                {
                    varType = (ushort)VariantType.VT_LPWSTR;
                    pwszVal = Marshal.StringToCoTaskMemUni(value as string);
                }
                else if (type == typeof(sbyte))
                {
                    varType = (ushort)VariantType.VT_I1;
                    cVal = (sbyte)value;
                }
                else if (type == typeof(byte))
                {
                    varType = (ushort)VariantType.VT_UI1;
                    bVal = (byte)value;
                }
                else if (type == typeof(System.Runtime.InteropServices.ComTypes.FILETIME))
                {
                    varType = (ushort)VariantType.VT_FILETIME;
                    filetime = (System.Runtime.InteropServices.ComTypes.FILETIME)value;
                }
                else if (value is char)
                {
                    varType = (ushort)VariantType.VT_LPSTR;
                    pszVal = Marshal.StringToCoTaskMemAnsi(new string(new char[] { (char)value }));
                }
                else if (type == typeof(short))
                {
                    varType = (ushort)VariantType.VT_I2;
                    iVal = (short)value;
                }
                else if (type == typeof(ushort))
                {
                    varType = (ushort)VariantType.VT_UI2;
                    uiVal = (ushort)value;
                }
                else if (type == typeof(int))
                {
                    varType = (ushort)VariantType.VT_I4;
                    intVal = (int)value;
                }
                else if (type == typeof(uint))
                {
                    varType = (ushort)VariantType.VT_UI4;
                    uintVal = (uint)value;
                }
                else if (type == typeof(Int64))
                {
                    varType = (ushort)VariantType.VT_I8;
                    lVal = (Int64)value;
                }
                else if (type == typeof(UInt64))
                {
                    varType = (ushort)VariantType.VT_UI8;
                    ulVal = (UInt64)value;
                }
                else if (value is float)
                {
                    varType = (ushort)VariantType.VT_R4;
                    fltVal = (float)value;
                }
                else if (value is double)
                {
                    varType = (ushort)VariantType.VT_R8;
                    dblVal = (double)value;
                }
                else if (value is Guid)
                {
                    var guid = ((Guid)value).ToByteArray();
                    varType = (ushort)VariantType.VT_CLSID;
                    pclsidVal = Marshal.AllocCoTaskMem(guid.Length);
                    Marshal.Copy(guid, 0, pclsidVal, guid.Length);
                }
                else if (value is bool)
                {
                    varType = (ushort)VariantType.VT_BOOL;
                    boolVal = (short)(((bool)value) ? -1 : 0);
                }
                else
                {
                    throw new System.NotSupportedException();
                }
            }
        }

        internal void InitVector(Array array, Type type, VariantType variantType)
        {
            Init(array, type, variantType | VariantType.VT_VECTOR);
        }

        /// <SecurityNote>
        /// Critical -Accesses unmanaged code and structure is overlapping in memory
        /// TreatAsSafe - inputs are verified or safe
        /// </SecurityNote>
        internal void Init(Array array, Type type, VariantType vt)
        {
            varType = (ushort)vt;
            ca.cElems = 0;
            ca.pElems = IntPtr.Zero;

            var length = array.Length;

            if (length > 0)
            {
                throw new NotImplementedException();
                //long size = Marshal.SizeOf(type) * length;

                //var destPtr = IntPtr.Zero;
                //var handle = new GCHandle();

                //try
                //{
                //    destPtr = Marshal.AllocCoTaskMem((int) size);
                //    handle = GCHandle.Alloc(array, GCHandleType.Pinned);

                //    // TODO: Fix copy to not use unsafe

                //    //Marshal.Copy();
                //    //unsafe
                //    //{
                //    //    CopyBytes((byte*)destPtr, (int)size, (byte*)handle.AddrOfPinnedObject(), (int)size);
                //    //}

                //    ca.cElems = (uint) length;
                //    ca.pElems = destPtr;

                //    destPtr = IntPtr.Zero;
                //}
                //finally
                //{
                //    if (handle.IsAllocated)
                //    {
                //        handle.Free();
                //    }

                //    if (destPtr != IntPtr.Zero)
                //    {
                //        Marshal.FreeCoTaskMem(destPtr);
                //    }
                //}
            }
        }

        /// <SecurityNote>
        /// Critical -Accesses unmanaged code and structure is overlapping in memory
        /// TreatAsSafe - inputs are verified or safe
        /// </SecurityNote>
        private void Init(string[] value, bool fAscii)
        {
            varType = (ushort)(fAscii ? VariantType.VT_LPSTR : VariantType.VT_LPWSTR);
            varType |= (ushort)VariantType.VT_VECTOR;
            ca.cElems = 0;
            ca.pElems = IntPtr.Zero;

            var length = value.Length;

            if (length > 0)
            {
                var destPtr = IntPtr.Zero;
                var sizeIntPtr = IntPtr.Size;
                long size = sizeIntPtr * length;
                var index = 0;

                try
                {
                    destPtr = Marshal.AllocCoTaskMem((int)size);

                    for (index = 0; index < length; index++)
                    {
                        IntPtr pString;
                        if (fAscii)
                        {
                            pString = Marshal.StringToCoTaskMemAnsi(value[index]);
                        }
                        else
                        {
                            pString = Marshal.StringToCoTaskMemUni(value[index]);
                        }
                        Marshal.WriteIntPtr(destPtr, (int)index * sizeIntPtr, pString);
                    }

                    ca.cElems = (uint)length;
                    ca.pElems = destPtr;
                    destPtr = IntPtr.Zero;
                }
                finally
                {
                    if (destPtr != IntPtr.Zero)
                    {
                        for (var i = 0; i < index; i++)
                        {
                            var pString = Marshal.ReadIntPtr(destPtr, i * sizeIntPtr);
                            Marshal.FreeCoTaskMem(pString);
                        }

                        Marshal.FreeCoTaskMem(destPtr);
                    }
                }
            }
        }
    }


    [ComImport]
    [Guid("7991EEC9-7E89-4D85-8390-6C703CEC60C0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMNotificationClient
    {
        /// <summary>
        /// The OnDeviceStateChanged method indicates that the state of an audio endpoint device has changed.
        /// </summary>
        /// <param name="deviceId">
        /// Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, 
        /// wide-character string containing the endpoint ID. The string remains valid for the duration of the call.
        /// </param>
        /// <param name="deviceState">
        /// Specifies the new state of the endpoint device. The value of this parameter is one of the following DEVICE_STATE_XXX constants:
        /// DEVICE_STATE_ACTIVE
        /// DEVICE_STATE_DISABLED
        /// DEVICE_STATE_NOTPRESENT
        /// DEVICE_STATE_UNPLUGGED
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        [PreserveSig]
        HResult OnDeviceStateChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, [In] Wasapi.Interop.DeviceState deviceState);

        /// <summary>
        /// The OnDeviceAdded method indicates that a new audio endpoint device has been added.
        /// </summary>
        /// <param name="deviceId">
        /// Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, 
        /// wide-character string containing the endpoint ID. The string remains valid for the duration of the call.
        /// </param>
        /// <returns></returns>
        [PreserveSig]
        HResult OnDeviceAdded([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        /// <summary>
        /// The OnDeviceRemoved method indicates that an audio endpoint device has been removed.
        /// </summary>
        /// <param name="deviceId">
        /// Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, 
        /// wide-character string containing the endpoint ID. The string remains valid for the duration of the call.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        [PreserveSig]
        HResult OnDeviceRemoved([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        /// <summary>
        /// The OnDefaultDeviceChanged method notifies the client that the default audio endpoint device for a particular device role has changed.
        /// </summary>
        /// <param name="dataFlow">
        ///  The data-flow direction of the endpoint device. This parameter is set to one of the following EDataFlow enumeration values:
        ///   - eRender
        ///   - eCapture
        /// The data-flow direction for a rendering device is eRender.The data-flow direction for a capture device is eCapture.
        /// </param>
        /// <param name="role">
        /// The device role of the audio endpoint device. This parameter is set to one of the following ERole enumeration values:
        ///   - eConsole
        ///   - eMultimedia
        ///   - eCommunications
        /// </param>
        /// <param name="deviceId">
        /// Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, 
        /// wide-character string containing the endpoint ID. The string remains valid for the duration of the call.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        [PreserveSig]
        HResult OnDefaultDeviceChanged([In] DataFlow dataFlow, [In] Role role, [In, MarshalAs(UnmanagedType.LPWStr)] string deviceId);

        /// <summary>
        /// The OnPropertyValueChanged method indicates that the value of a property belonging to an audio endpoint device has changed.
        /// </summary>
        /// <param name="deviceId">
        /// Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, 
        /// wide-character string containing the endpoint ID. The string remains valid for the duration of the call.
        /// </param>
        /// <param name="key">
        /// A PROPERTYKEY structure that specifies the property. The structure contains the property-set GUID and an index identifying a 
        /// property within the set. The structure is passed by value. It remains valid for the duration of the call. For more information
        /// about PROPERTYKEY, see the Windows SDK documentation.
        /// </param>
        /// <returns>
        /// If the method succeeds, it returns S_OK. If it fails, it returns an error code.
        /// </returns>
        [PreserveSig]
        HResult OnPropertyValueChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, [In] PropertyKey key);
    }
}
