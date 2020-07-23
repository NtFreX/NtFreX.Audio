namespace NtFreX.Audio.Wasapi.Interop
{
    /// <summary>
    /// The AUDCLNT_SHAREMODE enumeration defines constants that indicate whether an audio stream will run in shared mode or in exclusive mode.
    /// https://docs.microsoft.com/en-us/windows/win32/api/Audiosessiontypes/ne-audiosessiontypes-audclnt_sharemode
    /// </summary>
    internal enum AudioClientShareMode
    {
        /// <summary>
        /// The audio stream will run in shared mode.
        /// </summary>
        Shared = 0,

        /// <summary>
        /// The audio stream will run in exclusive mode.
        /// </summary>
        Exclusive = 1,
    }
}
