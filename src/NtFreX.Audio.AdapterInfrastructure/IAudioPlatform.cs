namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioPlatform
    {
        IAudioDeviceFactory AudioDeviceFactory { get; }
    }
}
