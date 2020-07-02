namespace NtFreX.Audio.Devices.Adapters
{
    internal interface IAudioDeviceAdapter
    {
        bool CanUse();
        IAudioDevice Initialize();
    }
}
