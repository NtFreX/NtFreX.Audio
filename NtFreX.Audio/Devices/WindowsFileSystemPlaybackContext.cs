using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    internal class WindowsFileSystemPlaybackContext : IDisposable
    {
        public string FileName { [return: NotNull] get; }
        public Process Process { [return: NotNull] get; }
        public TaskCompletionSource<int> CompletionSource { [return:NotNull] get; }

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
