using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;
using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Alsa
{
    public sealed class AlsaAudioPlatform : IAudioPlatform
    {
        public IAudioDeviceFactory AudioDeviceFactory { get; } = new AlsaDeviceFactory();

        public IAudioClientFactory AudioClientFactory { get; } = new AlsaAudioClientFactory();
    }

    public class AlsaAudioClientFactory : IAudioClientFactory
    {
        public AudioFormat GetDefaultFormat(IAudioDevice device)
        {
            throw new System.NotImplementedException();
        }

        public bool TryInitialize(AudioFormat format, IAudioDevice device, out IAudioClient? client, out AudioFormat supportedFormat)
        {
            if (!(device is AlsaDevice alsaDevice) || alsaDevice == null)
            {
                throw new ArgumentException($"Only {typeof(AlsaDevice).FullName} are supported", nameof(device));
            }

            var managedClient = new ManagedAlsaAudioClient();
            if(!managedClient.TryInitialize(alsaDevice.GetManagedDevice(), format, out supportedFormat))
            {
                client = null;
                return false;
            }

            client = new AlsaAudioClient(managedClient, alsaDevice);
            supportedFormat = format;
            return true;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
    internal class ManagedAlsaAudioRender
    {
        private readonly ManagedAlsaAudioClient managedAlsaAudioClient;
        private readonly ManagedAlsaDevice device;
        private readonly Task pumpTask;

        public ManagedAlsaAudioRender(ManagedAlsaAudioClient managedAlsaAudioClient, ManagedAlsaDevice device, CancellationToken cancellationToken)
        {
            this.managedAlsaAudioClient = managedAlsaAudioClient;
            this.device = device;

            pumpTask = Task.Run(PumpAudio, cancellationToken);
        }

        private async Task PumpAudio()
        {

        }
    }
    internal class ManagedAlsaAudioClient : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool TryInitialize(ManagedAlsaDevice device, AudioFormat format, out AudioFormat supportedFormat)
        {
            Interop.Alsa.snd_pcm_hw_params_malloc(out var parameters).ThrowIfNotSucceded("Can not allocate parameters object.");

            Interop.Alsa.snd_pcm_hw_params_any(device, parameters).ThrowIfNotSucceded("Can not fill parameters object.");

            Interop.Alsa.snd_pcm_hw_params_set_access(device, parameters, Interop.snd_pcm_access_t.SND_PCM_ACCESS_RW_INTERLEAVED).ThrowIfNotSucceded("Can not set access mode.");

            var result = (format.BitsPerSample / 8) switch
            {
                1 => Interop.Alsa.snd_pcm_hw_params_set_format(device, parameters, Interop.snd_pcm_format_t.SND_PCM_FORMAT_U8),
                2 => Interop.Alsa.snd_pcm_hw_params_set_format(device, parameters, Interop.snd_pcm_format_t.SND_PCM_FORMAT_S16_LE),
                3 => Interop.Alsa.snd_pcm_hw_params_set_format(device, parameters, Interop.snd_pcm_format_t.SND_PCM_FORMAT_S24_LE),
                _ => throw new Exception("Bits per sample error. Please reset the value of RecordingBitsPerSample."),
            };
            result.ThrowIfNotSucceded("Can not set format.");

            Interop.Alsa.snd_pcm_hw_params_set_channels(device, parameters, format.Channels).ThrowIfNotSucceded("Can not set channel.");

            var sampleRate = format.SampleRate;
            Interop.Alsa.snd_pcm_hw_params_set_rate_near(device, parameters, ref sampleRate, out var dir).ThrowIfNotSucceded("Can not set rate.");
            // TODO: find correct supported format
            supportedFormat = new AudioFormat(sampleRate, format.BitsPerSample, format.Channels, format.Type);

            Interop.Alsa.snd_pcm_hw_params(device, parameters).ThrowIfNotSucceded("Can not set hardware parameters.");
            return true;
        }
    }

    public class AlsaRenderContext : IRenderContext
    {
        private readonly ManagedAlsaAudioRender managedAlsaAudioRender;

        public Observable<EventArgs> EndOfDataReached => throw new NotImplementedException();

        public Observable<EventArgs> EndOfPositionReached => throw new NotImplementedException();

        public Observable<EventArgs<double>> PositionChanged => throw new NotImplementedException();

        internal AlsaRenderContext(ManagedAlsaAudioRender managedAlsaAudioRender)
        {
            this.managedAlsaAudioRender = managedAlsaAudioRender;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class AlsaAudioClient : IAudioClient
    {
        private readonly ManagedAlsaAudioClient managedAlsaAudioClient;
        private readonly AlsaDevice device;

        internal AlsaAudioClient(ManagedAlsaAudioClient managedAlsaAudioClient, AlsaDevice device)
        {
            this.managedAlsaAudioClient = managedAlsaAudioClient;
            this.device = device;
        }

        [return: NotNull]
        public Task<ICaptureContext> CaptureAsync(IAudioSink sink, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        [return: NotNull]
        public Task<IRenderContext> RenderAsync(IWaveAudioContainer audio, CancellationToken cancellationToken = default)
        {
            var render = new ManagedAlsaAudioRender(managedAlsaAudioClient, device.GetManagedDevice(), cancellationToken);
            return Task.FromResult(new AlsaRenderContext(render) as IRenderContext);
        }

        public void Dispose()
        {
            managedAlsaAudioClient.Dispose();
        }
    }
}
