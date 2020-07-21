using System;

namespace NtFreX.Audio.Alsa.Wrapper
{
    internal class ManagedAlsaDevice : IDisposable
    {
        private readonly IntPtr ptr;

        public string Id { get; }

        public ManagedAlsaDevice(IntPtr ptr, string id)
        {
            this.ptr = ptr;
            Id = id;
        }

        public void Dispose()
        {
            // TODO: dispose?
        }
    }
}
