using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public sealed class FileWaveAudioSink : StreamWaveAudioSink
    {
        private FileWaveAudioSink(string path, FileMode mode, IAudioSink innerSink)
            : base(File.Open(path, mode), innerSink) { }

        public static async Task<FileWaveAudioSink> CreateAsync(string path, IAudioFormat format, FileMode mode = FileMode.Create, IAudioSink? innerSink = null)
        {
            var sink = new FileWaveAudioSink(path, mode, innerSink ?? new VoidAudioSink());
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
