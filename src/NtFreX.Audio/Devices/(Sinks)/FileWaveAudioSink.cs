using System;
using System.IO;

namespace NtFreX.Audio.Devices
{
    public sealed class FileWaveAudioSink : StreamWaveAudioSink, IDisposable
    {
        public FileWaveAudioSink(string path, FileMode mode = FileMode.Create)
            : base(File.Open(path, mode)) { }

        public void Dispose()
        {
            Stream.Flush();
            Stream.Dispose();
        }
    }
}
