using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Samplers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class AudioDeviceExtensions
    {
        public static async Task<IRenderContext> RenderAsync(this IAudioDevice device, IWaveAudioContainer audio, CancellationToken cancellationToken)
        {
            _ = audio ?? throw new ArgumentNullException(nameof(audio));

            var audioPlatform = AudioEnvironment.Platform.Get();

#pragma warning disable CA2000 // Dispose objects before losing scope => IRenderContext wraps the client and disposes it
            if (!audioPlatform.AudioClientFactory.TryInitialize(audio.Format, device, out IAudioClient? audioClient, out var supportedFormat) || audioClient == null)
            {
                audio = await SampleAsync(audio, supportedFormat, cancellationToken).ConfigureAwait(false);

                if (!audioPlatform.AudioClientFactory.TryInitialize(audio.Format, device, out audioClient, out _) || audioClient == null)
                {
                    throw new Exception("The given audio is not supported");
                }
            }
#pragma warning restore CA2000 // Dispose objects before losing scope

            return await audioClient.RenderAsync(audio, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<ICaptureContext> CaptureAsync(this IAudioDevice device, AudioFormat format, IAudioSink sink, CancellationToken cancellationToken)
        {
            var audioPlatform = AudioEnvironment.Platform.Get();
#pragma warning disable CA2000 // Dispose objects before losing scope => ICaptureContext wraps the client and disposes it
            if (!audioPlatform.AudioClientFactory.TryInitialize(format, device, out var audioClient, out _) || audioClient == null)
            {
                throw new Exception("The given format is not supported");
            }
#pragma warning restore CA2000 // Dispose objects before losing scope

            return await audioClient.CaptureAsync(sink, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<IWaveAudioContainer> SampleAsync(IWaveAudioContainer source, IAudioFormat targetFormat, CancellationToken cancellationToken)
        {
            var pipe = new AudioSamplerPipe()
                    .Add(x => x.BitsPerSampleAudioSampler(targetFormat.BitsPerSample))
                    .Add(x => x.SampleRateAudioSampler(targetFormat.SampleRate))
                    // TODO: better channel sampler
                    .Add(x => x.ToMonoAudioSampler())
                    .Add(x => x.FromMonoAudioSampler(targetFormat.Channels));

            if (source.Format.Type != targetFormat.Type)
            {
                pipe.Add(factory => GetFormatTypeSampler(source.Format.Type, targetFormat.Type, factory));
            }

            return await pipe
                .RunAsync(source.AsEnumerable(cancellationToken), cancellationToken)
                .ConfigureAwait(false);
        }

        private static AudioSampler GetFormatTypeSampler(AudioFormatType sourceType, AudioFormatType targetType, AudioSamplerFactory audioSamplerFactory)
        {
            return (sourceType, targetType) switch
            {
                (AudioFormatType.Pcm, AudioFormatType.IeeFloat) => audioSamplerFactory.PcmToFloatAudioSampler(),
                (AudioFormatType.IeeFloat, AudioFormatType.Pcm) => audioSamplerFactory.FloatToPcmAudioSampler(),
                _ => throw new ArgumentException("The given format type is not supported"),
            };
        }
    }
}
