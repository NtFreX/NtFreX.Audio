using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Wrapper;
using System;
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
        public Task<IPlaybackContext> PlayAsync(IWaveAudioContainer audio, CancellationToken cancellationToken = default)
        {
            var audioRender = audioClient.GetAudioRenderer(audio, cancellationToken);
            var context = new WasapiPlaybackContext(audioRender);
            return Task.FromResult(context as IPlaybackContext);
        }

        public void Dispose()
        {
            audioClient.Dispose();
        }
    }
}
