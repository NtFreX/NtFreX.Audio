using NtFreX.Audio.Devices;
using NtFreX.Audio.Extensions;
using System.Globalization;
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
            var time = int.Parse(System.Console.ReadLine(), NumberFormatInfo.InvariantInfo) * 1000;

            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultCaptureDevice();

            var format = audioPlatform.AudioClientFactory.GetDefaultFormat(device);
           
            using var sink = new FileAudioSink(file);
            await sink.InitializeAsync(format).ConfigureAwait(false);

            (var context, var client) = await device.CaptureAsync(format, sink, cancellationToken).ConfigureAwait(false);

            await Task.Delay(time).ConfigureAwait(false);

            context.Dispose();
            client.Dispose();

            sink.Finish();

            System.Console.WriteLine();
            System.Console.WriteLine("  Audio device has been disposed");
        }
    }
}
