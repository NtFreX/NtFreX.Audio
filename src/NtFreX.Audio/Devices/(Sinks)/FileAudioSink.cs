using System;
using System.IO;

namespace NtFreX.Audio.Devices
{
    public sealed class FileAudioSink : StreamAudioSink, IDisposable
    {
        public FileAudioSink(string path, FileMode mode = FileMode.Create)
            : base(File.Open(path, mode)) { }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
