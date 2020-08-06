using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Wasapi.Wrapper;
using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi
{
    //TODO: cancelable
    public sealed class WasapiRenderContext : IRenderContext
    {
        private readonly ManagedAudioRender managedAudioRender;

        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs> EndOfPositionReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs<double>> PositionChanged { get; } = new Observable<EventArgs<double>>();

        internal WasapiRenderContext(ManagedAudioRender managedAudioRender)
        {
            this.managedAudioRender = managedAudioRender;

            managedAudioRender.EndOfDataReached.Subscribe(async (obj, args) => await EndOfDataReached.InvokeAsync(obj, args).ConfigureAwait(false));
            managedAudioRender.EndOfPositionReached.Subscribe(async (obj, args) => await EndOfPositionReached.InvokeAsync(obj, args).ConfigureAwait(false));
            managedAudioRender.PositionChanged.Subscribe(async (obj, args) => await PositionChanged.InvokeAsync(obj, args).ConfigureAwait(false));
        }

        public async ValueTask DisposeAsync()
        {
            await managedAudioRender.DisposeAsync().ConfigureAwait(false);

            EndOfDataReached.Dispose();
            PositionChanged.Dispose();
            EndOfPositionReached.Dispose();
        }

        public void Stop() => managedAudioRender.Stop();
        public void Start() => managedAudioRender.Start();
        public TimeSpan GetPosition() => managedAudioRender.GetPosition();

        private async Task OnPositionChanged(object sender, EventArgs<double> args) => await PositionChanged.InvokeAsync(sender, args).ConfigureAwait(false);
        private async Task OnEndOfDataReached(object sender, EventArgs args) => await EndOfDataReached.InvokeAsync(sender, args).ConfigureAwait(false);
        private async Task OnEndOfPositionReached(object sender, EventArgs args) => await EndOfPositionReached.InvokeAsync(sender, args).ConfigureAwait(false);
    }
}
