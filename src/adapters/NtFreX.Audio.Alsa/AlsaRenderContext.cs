using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;
using System;

namespace NtFreX.Audio.Alsa
{
    public sealed class AlsaRenderContext : IRenderContext
    {
        private readonly ManagedAlsaAudioRender managedAlsaAudioRender;

        public Observable<EventArgs> EndOfDataReached => throw new NotImplementedException();
        public Observable<EventArgs> EndOfPositionReached => throw new NotImplementedException();
        public Observable<EventArgs<double>> PositionChanged => throw new NotImplementedException();

        internal AlsaRenderContext(ManagedAlsaAudioRender managedAlsaAudioRender)
        {
            this.managedAlsaAudioRender = managedAlsaAudioRender;
        }

        public void Dispose() { }

        public void Stop() => managedAlsaAudioRender.Stop();
        public void Start() => managedAlsaAudioRender.Start();

        public TimeSpan GetPosition()
        {
            throw new NotImplementedException();
        }
    }
}
