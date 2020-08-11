using NtFreX.Audio.Infrastructure;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public sealed class FileWaveAudioSink : StreamWaveAudioSink
    {
        private FileWaveAudioSink(string path, FileMode mode)
            : base(File.Open(path, mode)) { }

        public static async Task<FileWaveAudioSink> CreateAsync(string path, IAudioFormat format, FileMode mode = FileMode.Create)
        {
            var sink = new FileWaveAudioSink(path, mode);
            await sink.InitializeAsync(format).ConfigureAwait(false);
            return sink;
        }

        protected override async ValueTask DisposeAsyncCore(bool disposing)
        {
            await base.DisposeAsyncCore(disposing).ConfigureAwait(false);

            Stream.Dispose();
        }
    }
}
