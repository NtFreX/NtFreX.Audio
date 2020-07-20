using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;
using System;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiPlaybackContext : IPlaybackContext
    {
        private readonly ManagedAudioRender managedAudioRender;

        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();

        internal WasapiPlaybackContext(ManagedAudioRender managedAudioRender)
        {
            this.managedAudioRender = managedAudioRender;

            managedAudioRender.EndOfDataReached.Subscribe(OnEndOfDataReached);
        }

        public void Dispose()
        {
            managedAudioRender.EndOfDataReached.Unsubscribe(OnEndOfDataReached);
            managedAudioRender.Dispose();
        }

        private void OnEndOfDataReached(object sender, EventArgs args)
            => EndOfDataReached.Invoke(sender, args);
    }
}
