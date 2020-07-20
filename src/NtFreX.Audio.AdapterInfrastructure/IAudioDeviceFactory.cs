namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioDeviceFactory
    {
        IAudioDevice GetDefaultRenderDevice();

        IAudioDevice GetDefaultCaptureDevice();
    }
}
