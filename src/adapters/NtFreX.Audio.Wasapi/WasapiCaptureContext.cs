using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Wrapper;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiCaptureContext : ICaptureContext
    {
        private readonly ManagedAudioCapture managedAudioCapturer;
        private readonly ManagedAudioClient managedAudioClient;

        internal WasapiCaptureContext(ManagedAudioCapture managedAudioCapture, ManagedAudioClient managedAudioClient)
        {
            this.managedAudioCapturer = managedAudioCapture;
            this.managedAudioClient = managedAudioClient;
        }

        public AudioFormat GetFormat() => managedAudioCapturer.GetFormat();

        public async ValueTask DisposeAsync()
        {
            await managedAudioCapturer.DisposeAsync().ConfigureAwait(false);
            managedAudioClient.Dispose();
        }
    }
}
