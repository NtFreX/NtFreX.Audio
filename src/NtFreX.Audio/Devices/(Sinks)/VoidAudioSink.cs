using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Containers.Serializers;
using NtFreX.Audio.Containers.Wave;
using NtFreX.Audio.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public class VoidAudioSink : IAudioSink
    {
        public void DataReceived(byte[] data) { }
    }
}
