using NtFreX.Audio.Containers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public class WindowsFileSystemAudioDevice : IAudioDevice
    {
        private readonly List<WindowsFileSystemPlaybackContext> _playbackContexts = new List<WindowsFileSystemPlaybackContext>();

        public static readonly string WindowsMediaPlayerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Windows Media Player\wmplayer.exe");

        public void Dispose()
        {
            foreach (var playbackContext in _playbackContexts)
            {
                playbackContext.Dispose();
            }
        }

        public Task Play(AudioContainer audio)
        {
            var fileExtension = AudioEnvironment.Serializer.GetPreferredFileExtension(audio);
            var fileName = $"temp{_playbackContexts.Count}.{fileExtension}";
            audio.ToFile(fileName);

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

            _playbackContexts.Add(new WindowsFileSystemPlaybackContext(fileName, process, completionSource));

            return completionSource.Task;
        }

        private void PlaybackProcess_Exited(object sender, EventArgs e)
        {
            var context = _playbackContexts.First(x => x.Process == sender);
            context.Process.Exited -= PlaybackProcess_Exited;
            context.CompletionSource.SetResult(context.Process.ExitCode);
            _playbackContexts.Remove(context);
        }
    }
}
