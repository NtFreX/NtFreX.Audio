using NtFreX.Audio.Infrastructure;
using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface ICaptureContext : IAsyncDisposable
    {
        AudioFormat GetFormat();
    }
}
