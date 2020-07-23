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

        private Task PumpAudioAsync() => Task.CompletedTask;
    }
}
