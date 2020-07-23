using NtFreX.Audio.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Converters
{
    internal interface IAudioConverter
    {
        Type From { get; }
        Type To { get; }

        Task<IAudioContainer> ConvertAsync(IStreamAudioContainer from, CancellationToken cancellationToken = default);
    }
}
