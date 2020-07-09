namespace NtFreX.Audio.Wasapi.Interop
{
    internal enum HResult : uint
    {
        /// <summary>
        /// Pointer that is not valid
        /// </summary>
        E_POINTER = 0x80004003,
        /// <summary>
        /// One or more arguments are not valid
        /// </summary>
        E_INVALIDARG = 0x80070057,
        /// <summary>
        /// Failed to allocate necessary memory
        /// </summary>
        E_OUTOFMEMORY = 0x8007000E
    }
}
