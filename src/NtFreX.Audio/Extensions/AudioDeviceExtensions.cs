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
            if (!audioPlatform.AudioClientFactory.TryInitialize(audio.Format, device, out audioClient, out var supportedFormat) || audioClient == null)
            {
                // TODO convert everyting nessesary (formatType)
                audio = await new AudioSamplerPipe()
                    .Add(x => x.BitsPerSampleAudioSampler(supportedFormat.BitsPerSample))
                    .Add(x => x.SampleRateAudioSampler(supportedFormat.SampleRate)) 
                    // TODO: better channel sampler
                    .Add(x => x.ToMonoAudioSampler())
                    .Add(x => x.FromMonoAudioSampler(supportedFormat.Channels))
                    .RunAsync(audio.AsEnumerable(cancellationToken), cancellationToken)
                    .ConfigureAwait(false);

                if (!audioPlatform.AudioClientFactory.TryInitialize(audio.Format, device, out audioClient, out _) || audioClient == null)
                {
                    throw new Exception("The given audio is not supported");
                }
            }

            var context = await audioClient.RenderAsync(audio, cancellationToken).ConfigureAwait(false);
            return (context, audioClient);
        }

        public static async Task<(ICaptureContext Context, IAudioClient Client)> CaptureAsync(this IAudioDevice device, AudioFormat format, IAudioSink sink, CancellationToken cancellationToken)
        {
            var audioPlatform = AudioEnvironment.Platform.Get();
            if (!audioPlatform.AudioClientFactory.TryInitialize(format, device, out var audioClient, out _) || audioClient == null)
            {
                throw new Exception("The given format is not supported");
            }

            var context = await audioClient.CaptureAsync(sink, cancellationToken).ConfigureAwait(false);
            return (context, audioClient);
        }
    }
}
