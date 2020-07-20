﻿using NtFreX.Audio.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IAudioClient : IDisposable
    {
        [return: NotNull] Task<IRenderContext> RenderAsync(IWaveAudioContainer audio, CancellationToken cancellationToken = default);
        [return: NotNull] Task<ICaptureContext> CaptureAsync(CancellationToken cancellationToken = default);
    }
}