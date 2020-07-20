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
        public static async Task<(IPlaybackContext, IAudioClient)> PlayAsync(this IAudioDevice device, IWaveAudioContainer audio, CancellationToken cancellationToken)
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

            var context = await audioClient.PlayAsync(audio, cancellationToken).ConfigureAwait(false);
            return (context, audioClient);
        }
    }
}
