using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    internal class CaptureAudioDemo : IDemo
    {
        public string Name => nameof(CaptureAudioDemo);

        public string Description => "Captures audio for a given time and saves it to a given file";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to create: ");
            var file = System.Console.ReadLine();

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            System.Console.Write("Enter the amount of seconds you want to record: ");
            var time = int.Parse(System.Console.ReadLine());

            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultCaptureDevice();

            (var context, var client) = await device.CaptureAsync(cancellationToken).ConfigureAwait(false);

            await Task.Delay(time).ConfigureAwait(false);

            await WaveEnumerableAudioContainerBuilder
                .Build(context.GetFormat(), context.GetSink())
                .ToFileAsync(file, FileMode.CreateNew, cancellationToken)
                .ConfigureAwait(false);

            context.Dispose();
            client.Dispose();

            System.Console.WriteLine();
            System.Console.WriteLine("  Audio device has been disposed");
        }
    }
}
