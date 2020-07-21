namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioSink
    {
        void DataReceived(byte[] data);
    }
}
