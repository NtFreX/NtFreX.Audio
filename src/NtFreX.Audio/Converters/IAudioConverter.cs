using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    internal interface IAudioConverter
    {
        Type From { get; }
        Type To { get; }

        Task<IAudioContainer> ConvertAsync(IAudioContainer from, CancellationToken cancellationToken = default);
    }
}
