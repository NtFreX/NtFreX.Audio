using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Samplers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class AudioDeviceExtensions
    {
        public static async Task<(IRenderContext Context, IAudioClient Client)> RenderAsync(this IAudioDevice device, IWaveAudioContainer audio, CancellationToken cancellationToken)
        {
            var audioPlatform = AudioEnvironment.Platform.Get();

            IAudioClient? audioClient;
            if (!audioPlatform.AudioClientFactory.TryInitialize(audio.ToFormat(), device, out audioClient, out var supportedFormat) || audioClient == null)
            {
                // TODO convert everyting nessesary
                audio = await new AudioSamplerPipe()
                    .Add(x => x.BitsPerSampleAudioSampler(supportedFormat.BitsPerSample))
                    .Add(x => x.SampleRateAudioSampler(supportedFormat.SampleRate))
                    .RunAsync(audio.AsEnumerable(cancellationToken), cancellationToken)
                    .ConfigureAwait(false);

                if (!audioPlatform.AudioClientFactory.TryInitialize(audio.ToFormat(), device, out audioClient, out _) || audioClient == null)
                {
                    throw new Exception("The given audio is not supported");
                }
            }

            var context = await audioClient.RenderAsync(audio, cancellationToken).ConfigureAwait(false);
            return (context, audioClient);
        }

        public static async Task<(ICaptureContext Context, IAudioClient Client)> CaptureAsync(this IAudioDevice device, CancellationToken cancellationToken)
        {
            var audioPlatform = AudioEnvironment.Platform.Get();
            var defaultFormat = audioPlatform.AudioClientFactory.GetDefaultFormat(device);

            if (!audioPlatform.AudioClientFactory.TryInitialize(defaultFormat, device, out var audioClient, out _) || audioClient == null)
            {
                throw new Exception();
            }

            var context = await audioClient.CaptureAsync(cancellationToken).ConfigureAwait(false);
            return (context, audioClient);
        }
    }
}
