using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Alsa.Wrapper;
using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Alsa
{
    public sealed class AlsaAudioClient : IAudioClient
    {
        private readonly AlsaDevice device;

        internal AlsaAudioClient(AlsaDevice device)
        {
            this.device = device;
        }

        [return: NotNull]
        public Task<ICaptureContext> CaptureAsync(IAudioSink sink, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        [return: NotNull]
        public Task<IRenderContext> RenderAsync(IAudioContainer audio, CancellationToken cancellationToken = default)
        {
            var render = new ManagedAlsaAudioRender(device.GetManagedDevice(), cancellationToken);
            return Task.FromResult(new AlsaRenderContext(render) as IRenderContext);
        }

        public void Dispose() { }
    }
}
