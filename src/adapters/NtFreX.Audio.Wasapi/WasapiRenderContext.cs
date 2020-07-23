using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;
using System;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiRenderContext : IRenderContext
    {
        private readonly ManagedAudioRender managedAudioRender;

        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();

        public Observable<EventArgs> EndOfPositionReached { get; } = new Observable<EventArgs>();

        public Observable<EventArgs<double>> PositionChanged { get; } = new Observable<EventArgs<double>>();

        internal WasapiRenderContext(ManagedAudioRender managedAudioRender)
        {
            this.managedAudioRender = managedAudioRender;

            managedAudioRender.EndOfDataReached.Subscribe(OnEndOfDataReached);
            managedAudioRender.EndOfPositionReached.Subscribe(OnEndOfPositionReached);
            managedAudioRender.PositionChanged.Subscribe(OnPositionChanged);
        }

        public void Dispose()
        {
            managedAudioRender.EndOfDataReached.Unsubscribe(OnEndOfDataReached);
            managedAudioRender.EndOfPositionReached.Unsubscribe(OnEndOfPositionReached);
            managedAudioRender.PositionChanged.Unsubscribe(OnPositionChanged);
            managedAudioRender.Dispose();
        }

        private void OnPositionChanged(object sender, EventArgs<double> args)
            => PositionChanged.Invoke(sender, args);

        private void OnEndOfDataReached(object sender, EventArgs args)
            => EndOfDataReached.Invoke(sender, args);

        private void OnEndOfPositionReached(object sender, EventArgs args)
            => EndOfPositionReached.Invoke(sender, args);
    }
}
