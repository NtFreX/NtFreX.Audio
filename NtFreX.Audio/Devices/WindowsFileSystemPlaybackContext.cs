using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    internal class WindowsFileSystemPlaybackContext : IDisposable
    {
        public string FileName { get; }
        public Process Process { get; }
        public TaskCompletionSource<int> CompletionSource { get; }

        internal WindowsFileSystemPlaybackContext(string fileName, Process process, TaskCompletionSource<int> completionSource)
        {
            FileName = fileName;
            Process = process;
            CompletionSource = completionSource;
        }

        public void Dispose()
            => File.Delete(FileName);
    }
}
