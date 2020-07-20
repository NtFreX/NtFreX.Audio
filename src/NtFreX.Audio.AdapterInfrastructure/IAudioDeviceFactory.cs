namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioDeviceFactory
    {
        IAudioDevice GetDefaultRenderDevice();
    }
}
