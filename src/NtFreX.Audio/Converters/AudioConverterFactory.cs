using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    public sealed class AudioConverterFactory
    {
        private readonly IAudioConverter[] converters = new IAudioConverter[]
        {
            new WaveToIntermediateConverter(),
            new IntermediateToWaveConverter()
        };

        public static AudioConverterFactory Instance { [return: NotNull] get; } = new AudioConverterFactory();

        private AudioConverterFactory() { }

        public async Task<TTo> ConvertAsync<TTo>(IAudioContainer audio, CancellationToken cancellationToken = default)
            where TTo: IAudioContainer
        {
            if (audio is TTo to)
            {
                return to;
            }

            var from = audio;
            if (audio is IntermediateListAudioContainer listContainer)
            {
                // TODO: this is ugly?
#pragma warning disable CA2000 // Dispose objects before losing scope => object is wrapped by another dispsable which cleans this up
                from = listContainer.AsEnumerable();
#pragma warning restore CA2000 // Dispose objects before losing scope
            }

            var converter = converters.FirstOrDefault(x => x.From.IsAssignableFrom(audio.GetType()) && x.To.FullName == typeof(TTo).FullName);
            if (converter == null)
            {
                throw new NotImplementedException("The given conversion is not supported");
            }

            return (TTo) await converter.ConvertAsync(from, cancellationToken).ConfigureAwait(false);
        }
    }
}
