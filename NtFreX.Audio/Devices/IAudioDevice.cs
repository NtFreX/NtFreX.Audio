using NtFreX.Audio.Containers;
using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public interface IAudioDevice : IDisposable
    {
        Task Play(AudioContainer audio);
    }
}
