namespace NtFreX.Audio.PulseAudio.Interop
{
    /// <summary>
    /// A list of channel labels.
    /// </summary>
    internal enum pa_channel_position
    {
        PA_CHANNEL_POSITION_INVALID,
        PA_CHANNEL_POSITION_MONO,
        /// <summary>
        /// Apple, Dolby call this 'Left'.
        /// </summary>
        PA_CHANNEL_POSITION_FRONT_LEFT,
        /// <summary>
        /// Apple, Dolby call this 'Right'.
        /// </summary>
        PA_CHANNEL_POSITION_FRONT_RIGHT,
        /// <summary>
        /// Apple, Dolby call this 'Center'.
        /// </summary>
        PA_CHANNEL_POSITION_FRONT_CENTER,
        /// <summary>
        /// Microsoft calls this 'Back Center', Apple calls this 'Center Surround', Dolby calls this 'Surround Rear Center'.
        /// </summary>
        PA_CHANNEL_POSITION_REAR_CENTER,
        /// <summary>
        /// Microsoft calls this 'Back Left', Apple calls this 'Left Surround' (!), Dolby calls this 'Surround Rear Left'.
        /// </summary>
        PA_CHANNEL_POSITION_REAR_LEFT,
        /// <summary>
        /// Microsoft calls this 'Back Right', Apple calls this 'Right Surround' (!), Dolby calls this 'Surround Rear Right'.
        /// </summary>
        PA_CHANNEL_POSITION_REAR_RIGHT,
        /// <summary>
        /// Microsoft calls this 'Low Frequency', Apple calls this 'LFEScreen'.
        /// </summary>
        PA_CHANNEL_POSITION_LFE,
        /// <summary>
        /// Apple, Dolby call this 'Left Center'.
        /// </summary>
        PA_CHANNEL_POSITION_FRONT_LEFT_OF_CENTER,
        /// <summary>
        /// Apple, Dolby call this 'Right Center.
        /// </summary>
        PA_CHANNEL_POSITION_FRONT_RIGHT_OF_CENTER,
        /// <summary>
        /// Apple calls this 'Left Surround Direct', Dolby calls this 'Surround Left' (!)
        /// </summary>
        PA_CHANNEL_POSITION_SIDE_LEFT,
        /// <summary>
        /// Apple calls this 'Right Surround Direct', Dolby calls this 'Surround Right' (!)
        /// </summary>
        PA_CHANNEL_POSITION_SIDE_RIGHT,
        PA_CHANNEL_POSITION_AUX0,
        PA_CHANNEL_POSITION_AUX1,
        PA_CHANNEL_POSITION_AUX2,
        PA_CHANNEL_POSITION_AUX3,
        PA_CHANNEL_POSITION_AUX4,
        PA_CHANNEL_POSITION_AUX5,
        PA_CHANNEL_POSITION_AUX6,
        PA_CHANNEL_POSITION_AUX7,
        PA_CHANNEL_POSITION_AUX8,
        PA_CHANNEL_POSITION_AUX9,
        PA_CHANNEL_POSITION_AUX10,
        PA_CHANNEL_POSITION_AUX11,
        PA_CHANNEL_POSITION_AUX12,
        PA_CHANNEL_POSITION_AUX13,
        PA_CHANNEL_POSITION_AUX14,
        PA_CHANNEL_POSITION_AUX15,
        PA_CHANNEL_POSITION_AUX16,
        PA_CHANNEL_POSITION_AUX17,
        PA_CHANNEL_POSITION_AUX18,
        PA_CHANNEL_POSITION_AUX19,
        PA_CHANNEL_POSITION_AUX20,
        PA_CHANNEL_POSITION_AUX21,
        PA_CHANNEL_POSITION_AUX22,
        PA_CHANNEL_POSITION_AUX23,
        PA_CHANNEL_POSITION_AUX24,
        PA_CHANNEL_POSITION_AUX25,
        PA_CHANNEL_POSITION_AUX26,
        PA_CHANNEL_POSITION_AUX27,
        PA_CHANNEL_POSITION_AUX28,
        PA_CHANNEL_POSITION_AUX29,
        PA_CHANNEL_POSITION_AUX30,
        PA_CHANNEL_POSITION_AUX31,
        /// <summary>
        /// Apple calls this 'Top Center Surround'.
        /// </summary>
        PA_CHANNEL_POSITION_TOP_CENTER,
        /// <summary>
        /// Apple calls this 'Vertical Height Left'.
        /// </summary>
        PA_CHANNEL_POSITION_TOP_FRONT_LEFT,
        /// <summary>
        /// Apple calls this 'Vertical Height Right'.
        /// </summary>
        PA_CHANNEL_POSITION_TOP_FRONT_RIGHT,
        /// <summary>
        /// Apple calls this 'Vertical Height Center'.
        /// </summary>
        PA_CHANNEL_POSITION_TOP_FRONT_CENTER,
        /// <summary>
        /// Microsoft and Apple call this 'Top Back Left'.
        /// </summary>
        PA_CHANNEL_POSITION_TOP_REAR_LEFT,
        /// <summary>
        /// Microsoft and Apple call this 'Top Back Right'.
        /// </summary>
        PA_CHANNEL_POSITION_TOP_REAR_RIGHT,
        /// <summary>
        /// Microsoft and Apple call this 'Top Back Center'.
        /// </summary>
        PA_CHANNEL_POSITION_TOP_REAR_CENTER,
        PA_CHANNEL_POSITION_MAX
    }
}
