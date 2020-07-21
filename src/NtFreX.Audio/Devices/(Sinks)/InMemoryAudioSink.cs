using NtFreX.Audio.AdapterInfrastructure;
using System.Collections.Generic;

namespace NtFreX.Audio.Devices
{
    public class InMemoryAudioSink : IAudioSink
    {
        private List<byte> sink = new List<byte>();

        public byte[] GetData() => sink.ToArray();

        public void DataReceived(byte[] data)
        {
            sink.AddRange(data);
        }
    }
}
