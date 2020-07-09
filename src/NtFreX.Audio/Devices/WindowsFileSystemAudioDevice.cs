using NtFreX.Audio.Containers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public sealed class WindowsFileSystemAudioDevice : IAudioDevice
    {
        public static readonly string WindowsMediaPlayerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Windows Media Player\wmplayer.exe");
        private readonly List<WindowsFileSystemPlaybackContext> playbackContexts = new List<WindowsFileSystemPlaybackContext>();

        public void Dispose()
        {
            foreach (var playbackContext in playbackContexts)
            {
                playbackContext.Dispose();
            }
        }

        [return:NotNull] public async Task<Task> PlayAsync([NotNull] IStreamAudioContainer audio)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var fileExtension = AudioEnvironment.Serializer.GetPreferredFileExtension(audio);
            var fileName = $"temp{playbackContexts.Count}.{fileExtension}";
            await audio.ToFileAsync(fileName).ConfigureAwait(false);

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = WindowsMediaPlayerPath,
                    Arguments = Path.Combine(Directory.GetCurrentDirectory(), fileName),
                    UseShellExecute = true
                }
            };
            process.EnableRaisingEvents = true;
            process.Exited += PlaybackProcess_Exited;
            process.Start();

            var completionSource = new TaskCompletionSource<int>();

            playbackContexts.Add(new WindowsFileSystemPlaybackContext(fileName, process, completionSource));

            return completionSource.Task;
        }

        private void PlaybackProcess_Exited(object? sender, EventArgs e)
        {
            var context = playbackContexts.First(x => x.Process == sender);
            context.Process.Exited -= PlaybackProcess_Exited;
            context.CompletionSource.SetResult(context.Process.ExitCode);
            playbackContexts.Remove(context);
        }
    }
}
