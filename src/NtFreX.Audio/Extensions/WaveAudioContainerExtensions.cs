using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Samplers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class WaveAudioContainerExtensions
    {
        public static WaveEnumerableAudioContainer AsEnumerable(this IWaveAudioContainer audio, CancellationToken cancellationToken = default)
        {
            //TODO: is this clean code?
            if (audio is WaveEnumerableAudioContainer waveEnumerableAudioContainer)
            {
                return waveEnumerableAudioContainer;
            }
            if (audio is WaveStreamAudioContainer waveStreamAudioContainer)
            {
                return waveStreamAudioContainer.ToEnumerable(cancellationToken);
            }

            throw new ArgumentException("The given wave container type is not supported", nameof(audio));
        }

        public static async Task<WaveEnumerableAudioContainer> ToFormatAsync(this IWaveAudioContainer source, IAudioFormat targetFormat, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = targetFormat ?? throw new ArgumentNullException(nameof(targetFormat));

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
