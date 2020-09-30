using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Alsa.Wrapper
{
    internal class ManagedAlsaAudioRender
    {
        private readonly ManagedAlsaDevice device;
        private readonly Task pumpTask;

        public ManagedAlsaAudioRender(ManagedAlsaDevice device, CancellationToken cancellationToken)
        {
            this.device = device;

            pumpTask = Task.Run(PumpAudioAsync, cancellationToken);
        }

        public bool IsStopped() => throw new NotImplementedException();
        public void Stop() => throw new NotImplementedException();
        public void Start() => throw new NotImplementedException();

        private Task PumpAudioAsync() => Task.CompletedTask;
    }
}
