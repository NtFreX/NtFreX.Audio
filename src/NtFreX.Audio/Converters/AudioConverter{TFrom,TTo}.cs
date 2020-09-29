using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    internal abstract class AudioConverter<TFrom, TTo> : IAudioConverter
        where TFrom : IAudioContainer
        where TTo : IAudioContainer
    {
        public Type From => typeof(TFrom);
        public Type To => typeof(TTo);

        public async Task<IAudioContainer> ConvertAsync(IAudioContainer from, CancellationToken cancellationToken = default)
            => await ConvertAsync((TFrom)from, cancellationToken).ConfigureAwait(false);

        protected abstract Task<TTo> ConvertAsync(TFrom from, CancellationToken cancellationToken = default);
    }
}
