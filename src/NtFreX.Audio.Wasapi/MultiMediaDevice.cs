using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;
using System;

namespace NtFreX.Audio.Wasapi
{
    public sealed class MultiMediaDevice : IAudioDevice
    {
        private readonly ManagedMultiMediaDevice wrapper;

        private MultiMediaDevice(ManagedMultiMediaDevice wrapper)
        {
            this.wrapper = wrapper;
        }

        public string GetId() => wrapper.GetId();

        internal static MultiMediaDevice FromManaged(ManagedMultiMediaDevice wrapper) => new MultiMediaDevice(wrapper);

        internal ManagedMultiMediaDevice ToManaged() => wrapper;

        public void Dispose()
        {
            wrapper.Dispose();
        }
    }
}
