using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
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

        public AudioFormat GetDefaultFormat()
        {
            var wrapper = ToManaged();
            // TODO: dot not dispose client only to get mix format?
            using var audioClient = wrapper.Activate();

            using var mixFormat = audioClient.GetMixFormat();

            return mixFormat?.ToAudioFormat() ?? throw new Exception();
        }

        public bool TryInitialize(IAudioFormat format, out IAudioClient? client, out IAudioFormat supportedFormat)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            var wrapper = ToManaged();
            var managedFormat = format.ToManagedWaveFormat();

#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            var audioClient = wrapper.Activate();
#pragma warning restore CA2000 // Dispose objects before losing scope

            var success = audioClient.TryInitialize(managedFormat, out var managedSupportedFormat);

            supportedFormat = managedSupportedFormat.ToAudioFormat();
            managedSupportedFormat.Dispose();

            client = success ? new WasapiAudioClient(audioClient) : null;
            return success;
        }

        public void Dispose()
        {
            wrapper.Dispose();
        }

        internal static MultiMediaDevice FromManaged(ManagedMultiMediaDevice wrapper) => new MultiMediaDevice(wrapper);

        internal ManagedMultiMediaDevice ToManaged() => wrapper;
    }
}
