using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    internal class WindowsFileSystemPlaybackContext : IDisposable
    {
        public string FileName { get; }
        public Process Process { get; }
        public TaskCompletionSource<int> CompletionSource { get; }

        internal WindowsFileSystemPlaybackContext([NotNull] string fileName, [NotNull] Process process, [NotNull] TaskCompletionSource<int> completionSource)
        {
            FileName = fileName;
            Process = process;
            CompletionSource = completionSource;
        }

        public void Dispose()
            => File.Delete(FileName);
    }
}
