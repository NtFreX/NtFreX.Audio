using NtFreX.Audio.Containers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public interface IAudioDevice : IDisposable
    {
         [return:NotNull] Task<Task> PlayAsync([NotNull] IStreamAudioContainer audio);
    }
}
