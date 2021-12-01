using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal class GetDefaulRenderDeviceDemo : IDemo
    {
        public string Name => nameof(GetDefaulRenderDeviceDemo);
        public string Description => "Loads the default render device and prits its id";

        public Task RunAsync(CancellationToken cancellationToken)
        {
            using var device = AudioDevice.GetDefaultRenderDevice();

            System.Console.WriteLine(device.GetId());

            return Task.CompletedTask;
        }
    }
}
