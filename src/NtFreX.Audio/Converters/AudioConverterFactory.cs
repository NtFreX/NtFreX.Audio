using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    //internal interface IAudioConverter
    //{
    //    Task<IAudioContainer> ConvertAsync(IStreamAudioContainer from);
    //}

    //internal abstract class AudioConverter<TFrom, TTo> : IAudioConverter
    //    where TFrom : IStreamAudioContainer
    //    where TTo : IAudioContainer
    //{
    //    public async Task<IAudioContainer> ConvertAsync(IStreamAudioContainer from)
    //        => await ConvertAsync((TFrom)from).ConfigureAwait(false);

    //    protected abstract Task<TTo> ConvertAsync(TFrom from);
    //}

    public sealed class AudioConverterFactory
    {
        public static AudioConverterFactory Instance { [return: NotNull] get; } = new AudioConverterFactory();

        private AudioConverterFactory() { }

        public TTo Convert<TTo>(IStreamAudioContainer audio)
            where TTo: IAudioContainer
        {
            if(audio is TTo to)
            {
                return to;
            }

            // TODO: implement
            throw new NotImplementedException();
        }
    }
}
