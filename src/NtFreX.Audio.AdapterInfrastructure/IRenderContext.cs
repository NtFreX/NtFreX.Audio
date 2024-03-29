﻿using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using System;

namespace NtFreX.Audio.AdapterInfrastructure
{
    public interface IRenderContext : IAsyncDisposable
    {
        Observable<EventArgs<Exception>> RenderExceptionOccured { get; }
        Observable<EventArgs> EndOfDataReached { get; }
        Observable<EventArgs> EndOfPositionReached { get; }
        Observable<EventArgs<double>> PositionChanged { get; }

        bool IsStopped();

#pragma warning disable CA1716 // Identifiers should not match keywords
        void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords
        void Start();

        IAudioFormat GetFormat();

        TimeSpan GetLength();
        TimeSpan GetPosition();
        void SetPosition(TimeSpan position);
    }
}
