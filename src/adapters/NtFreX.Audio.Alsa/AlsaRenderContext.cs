﻿using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;
using NtFreX.Audio.Infrastructure;
using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Alsa
{
    public sealed class AlsaRenderContext : IRenderContext
    {
        private readonly ManagedAlsaAudioRender managedAlsaAudioRender;

        public Observable<EventArgs<Exception>> RenderExceptionOccured => throw new NotImplementedException();
        public Observable<EventArgs> EndOfDataReached => throw new NotImplementedException();
        public Observable<EventArgs> EndOfPositionReached => throw new NotImplementedException();
        public Observable<EventArgs<double>> PositionChanged => throw new NotImplementedException();

        internal AlsaRenderContext(ManagedAlsaAudioRender managedAlsaAudioRender)
        {
            this.managedAlsaAudioRender = managedAlsaAudioRender;
        }

        public ValueTask DisposeAsync() => new ValueTask(Task.CompletedTask);

        public bool IsStopped() => managedAlsaAudioRender.IsStopped();
        public void Stop() => managedAlsaAudioRender.Stop();
        public void Start() => managedAlsaAudioRender.Start();

        public TimeSpan GetPosition()
        {
            throw new NotImplementedException();
        }

        public void SetPosition(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        public IAudioFormat GetFormat()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetLength()
        {
            throw new NotImplementedException();
        }
    }
}
