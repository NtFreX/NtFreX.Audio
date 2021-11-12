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

        public Observable<EventArgs<Exception>> RenderExceptionOccured { get; } = new Observable<EventArgs<Exception>>();
        public Observable<EventArgs> EndOfDataReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs> EndOfPositionReached { get; } = new Observable<EventArgs>();
        public Observable<EventArgs<double>> PositionChanged { get; } = new Observable<EventArgs<double>>();

        internal WasapiRenderContext(ManagedAudioRender managedAudioRender, ManagedAudioClient managedAudioClient)
        {
            this.managedAudioRender = managedAudioRender;
            this.managedAudioClient = managedAudioClient;

            managedAudioRender.RenderExceptionOccured.Subscribe(async (obj, args) => await RenderExceptionOccured.InvokeAsync(obj, args).ConfigureAwait(false));
            managedAudioRender.EndOfDataReached.Subscribe(async (obj, args) => await EndOfDataReached.InvokeAsync(obj, args).ConfigureAwait(false));
            managedAudioRender.EndOfPositionReached.Subscribe(async (obj, args) => await EndOfPositionReached.InvokeAsync(obj, args).ConfigureAwait(false));
            managedAudioRender.PositionChanged.Subscribe(async (obj, args) => await PositionChanged.InvokeAsync(obj, args).ConfigureAwait(false));
        }

        public async ValueTask DisposeAsync()
        {
            await managedAudioRender.DisposeAsync().ConfigureAwait(false);
            managedAudioClient.Dispose();

            RenderExceptionOccured.Dispose();
            EndOfDataReached.Dispose();
            PositionChanged.Dispose();
            EndOfPositionReached.Dispose();
        }

        public bool IsStopped() => managedAudioRender.IsStopped();
        public void Stop() => managedAudioRender.Stop();
        public void Start() => managedAudioRender.Start();
        public TimeSpan GetLength() => managedAudioRender.GetLength();
        public TimeSpan GetPosition() => managedAudioRender.GetPosition();
        public void SetPosition(TimeSpan position) => managedAudioRender.SetPosition(position);
        public IAudioFormat GetFormat() => managedAudioClient.InitializedFormat?.ToAudioFormat() ?? throw new Exception("No audio format is initialized");
    }
}
