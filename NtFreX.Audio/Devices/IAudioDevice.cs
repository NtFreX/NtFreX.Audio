using NtFreX.Audio.Containers;
using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public interface IAudioDevice : IDisposable
    {
        Task<Task> PlayAsync(AudioContainer audio);
    }
}
