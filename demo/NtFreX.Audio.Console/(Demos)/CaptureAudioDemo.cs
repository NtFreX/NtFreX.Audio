using NtFreX.Audio.Devices;
using NtFreX.Audio.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal class CaptureAudioDemo : IDemo
    {
        public string Name => nameof(CaptureAudioDemo);

        public string Description => "Captures audio for a given time and saves it to a given file";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to create: ");
            var file = System.Console.ReadLine();
            _ = file ?? throw new Exception("Enter a valid file name");

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            System.Console.Write("Enter the amount of seconds you want to record: ");
            var time = int.Parse(System.Console.ReadLine() ?? string.Empty, NumberFormatInfo.InvariantInfo) * 1000;

            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultCaptureDevice();

            var format = audioPlatform.AudioClientFactory.GetDefaultFormat(device);
            AudioFactory.PrintAudioFormat(format);

            await using var sink = await FileWaveAudioSink.CreateAsync(file, format).ConfigureAwait(false);

            await using var context = await device.CaptureAsync(format, sink, cancellationToken).ConfigureAwait(false);

            try
            {
                await Task.Delay(time, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                System.Console.WriteLine();
                System.Console.WriteLine("  Audio device has been disposed");
            }
        }
    }
}
