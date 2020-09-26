using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Samplers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Extensions
{
    public static class AudioContainerExtensions
    {
        public static async Task<IntermediateEnumerableAudioContainer> RunAudioPipeAsync(this Task<IntermediateEnumerableAudioContainer> containerTask, AudioSamplerPipe pipe, CancellationToken cancellationToken)
        {
            _ = containerTask ?? throw new ArgumentNullException(nameof(containerTask));
            _ = pipe ?? throw new ArgumentNullException(nameof(pipe));

            var container = await containerTask.ConfigureAwait(false);
            return await RunAudioPipeAsync(container, pipe, cancellationToken).ConfigureAwait(false);
        }

        public static Task<IntermediateEnumerableAudioContainer> RunAudioPipeAsync(this IntermediateEnumerableAudioContainer container, AudioSamplerPipe pipe, CancellationToken cancellationToken)
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));
            _ = pipe ?? throw new ArgumentNullException(nameof(pipe));

            return pipe.RunAsync(container, cancellationToken);
        }

        public static async Task<T> ConvertAsync<T>(this Task<IAudioContainer> container, CancellationToken cancellationToken)
            where T : IAudioContainer
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));

            return await ConvertAsync<T>(await container.ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        public static Task<T> ConvertAsync<T>(this IAudioContainer container, CancellationToken cancellationToken)
            where T : IAudioContainer
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));

            return AudioEnvironment.Converter.ConvertAsync<T>(container, cancellationToken);
        }

        public static async Task<T> ToFileAsync<T>(this Task<T> container, string path, FileMode fileMode, CancellationToken cancellationToken)
            where T : IAudioContainer
        {
            _ = container ?? throw new ArgumentNullException(nameof(container));

            return await (await container.ConfigureAwait(false)).ToFileAsync(path, fileMode, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<T> ToFileAsync<T>(this T container, string path, FileMode fileMode, CancellationToken cancellationToken)
            where T : IAudioContainer
        {
            using var stream = new FileStream(path, fileMode);

            await AudioEnvironment.Serializer
                .ToStreamAsync(container, stream, cancellationToken)
                .ConfigureAwait(false);

            return container;
        }

        public static async Task<IntermediateEnumerableAudioContainer> ToFormatAsync(this IAudioContainer source, IAudioFormat targetFormat, CancellationToken cancellationToken = default)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = targetFormat ?? throw new ArgumentNullException(nameof(targetFormat));

            var pipe = new AudioSamplerPipe()
                    .Add(x => x.BitsPerSampleAudioSampler(targetFormat.BitsPerSample))
                    .Add(x => x.SampleRateAudioSampler(targetFormat.SampleRate))
                    // TODO: better channel sampler
                    .Add(x => x.ToMonoAudioSampler())
                    .Add(x => x.FromMonoAudioSampler(targetFormat.Channels));

            var format = source.GetFormat();
            if (format.Type != targetFormat.Type)
            {
                pipe.Add(factory => GetFormatTypeSampler(format.Type, targetFormat.Type, factory));
            }

            if(source is IntermediateListAudioContainer listContainer)
            {
                // TODO: this is ugly?
                source = listContainer.AsEnumerable();
            }

            var intermediate = await AudioEnvironment.Converter
                .ConvertAsync<IntermediateEnumerableAudioContainer>(source, cancellationToken)
                .ConfigureAwait(false);

            return await pipe
                .RunAsync(intermediate, cancellationToken)
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
