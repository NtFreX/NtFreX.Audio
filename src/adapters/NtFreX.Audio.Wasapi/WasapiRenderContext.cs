using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Wasapi.Wrapper;
using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Wasapi
{
    public sealed class WasapiRenderContext : IRenderContext
    {
        private readonly ManagedAudioRender managedAudioRender;
        private readonly ManagedAudioClient managedAudioClient;

        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs> EndOfPositionReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs<double>> PositionChanged { get; } = new Observable<EventArgs<double>>();

        internal WasapiRenderContext(ManagedAudioRender managedAudioRender, ManagedAudioClient managedAudioClient)
        {
            this.managedAudioRender = managedAudioRender;
            this.managedAudioClient = managedAudioClient;
            
            managedAudioRender.EndOfDataReached.Subscribe(async (obj, args) => await EndOfDataReached.InvokeAsync(obj, args).ConfigureAwait(false));
            managedAudioRender.EndOfPositionReached.Subscribe(async (obj, args) => await EndOfPositionReached.InvokeAsync(obj, args).ConfigureAwait(false));
            managedAudioRender.PositionChanged.Subscribe(async (obj, args) => await PositionChanged.InvokeAsync(obj, args).ConfigureAwait(false));
        }

        public async ValueTask DisposeAsync()
        {
            await managedAudioRender.DisposeAsync().ConfigureAwait(false);
            managedAudioClient.Dispose();

            EndOfDataReached.Dispose();
            PositionChanged.Dispose();
            EndOfPositionReached.Dispose();
        }

        public void Stop() => managedAudioRender.Stop();
        public void Start() => managedAudioRender.Start();
        public TimeSpan GetPosition() => managedAudioRender.GetPosition();
        public void SetPosition(TimeSpan position) => managedAudioRender.SetPosition(position);
        public IAudioFormat GetFormat() => managedAudioClient.InitializedFormat?.ToAudioFormat() ?? throw new Exception("No audio format is initialized");

        private async Task OnPositionChanged(object sender, EventArgs<double> args) => await PositionChanged.InvokeAsync(sender, args).ConfigureAwait(false);
        private async Task OnEndOfDataReached(object sender, EventArgs args) => await EndOfDataReached.InvokeAsync(sender, args).ConfigureAwait(false);
        private async Task OnEndOfPositionReached(object sender, EventArgs args) => await EndOfPositionReached.InvokeAsync(sender, args).ConfigureAwait(false);
    }
}
