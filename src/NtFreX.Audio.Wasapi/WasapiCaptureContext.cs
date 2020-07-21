using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiCaptureContext : ICaptureContext
    {
        private readonly ManagedAudioCapture managedAudioCapturer;

        internal WasapiCaptureContext(ManagedAudioCapture managedAudioCapture)
        {
            this.managedAudioCapturer = managedAudioCapture;
        }

        public AudioFormat GetFormat() => managedAudioCapturer.GetFormat();

        public void Dispose()
        {
            managedAudioCapturer.Dispose();
        }
    }
}
