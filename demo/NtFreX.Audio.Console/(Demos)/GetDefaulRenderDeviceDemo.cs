using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    internal class GetDefaulRenderDeviceDemo : IDemo
    {
        public string Name => nameof(GetDefaulRenderDeviceDemo);
        public string Description => "Loads the default render device and prits its id";

        public Task RunAsync(CancellationToken cancellationToken)
        {
            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultRenderDevice();

            System.Console.WriteLine(device.GetId());

            return Task.CompletedTask;
        }
    }
}
