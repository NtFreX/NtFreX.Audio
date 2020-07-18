using System;
using System.Runtime.InteropServices;

namespace NtFreX.Audio.Wasapi.Interop
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct PropVariant
    {
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
    }
}
