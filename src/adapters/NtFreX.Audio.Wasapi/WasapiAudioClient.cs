using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Wasapi.Wrapper;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiAudioClient : IAudioClient
    {
        private readonly ManagedAudioClient audioClient;

        internal WasapiAudioClient(ManagedAudioClient audioClient)
        {
            this.audioClient = audioClient;
        }

        [return: NotNull]
        public Task<IRenderContext> RenderAsync(IWaveAudioContainer audio, CancellationToken cancellationToken = default)
        {
            var audioRender = audioClient.GetAudioRenderer(audio, cancellationToken);
            var context = new WasapiRenderContext(audioRender, audioClient);
            return Task.FromResult(context as IRenderContext);
        }

        public Task<ICaptureContext> CaptureAsync(IAudioSink sink, CancellationToken cancellationToken = default)
        {
            var audioCapturer = audioClient.GetAudioCapture(sink, cancellationToken);
            var context = new WasapiCaptureContext(audioCapturer, audioClient);
            return Task.FromResult(context as ICaptureContext);
        }

        public void Dispose()
        {
            audioClient.Dispose();
        }
    }
}
