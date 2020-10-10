using NtFreX.Audio.Devices;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.SpeechRecognition;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal class SpeechRecognitionDemo : IDemo
    {
        public string Name => nameof(SpeechRecognitionDemo);

        public string Description => "Recognizes speech for a given time and saves it to a given text file";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var speechModel = ConsoleHelper.AquireFile("Enter the file with the speech model: ");
            if (!File.Exists(speechModel))
            {
                var speechRecognizer = SpeechRecognizer.Initialize();
                await speechRecognizer.NeuralNetwork.SaveToFileAsync(speechModel).ConfigureAwait(false);
                System.Console.WriteLine("The given speech model does not exist. A new one was created.");
            }

            var outputFile = ConsoleHelper.AquireNewFile("Enter the file to write to: ");

            System.Console.Write("Enter the amount of seconds you want to record: ");
            var time = int.Parse(System.Console.ReadLine() ?? string.Empty, NumberFormatInfo.InvariantInfo) * 1000;

            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultCaptureDevice();

            var format = audioPlatform.AudioClientFactory.GetDefaultFormat(device);
            AudioFactory.PrintAudioFormat(format);

            await using var fileSink = await FileWaveAudioSink.CreateAsync(outputFile, format).ConfigureAwait(false);
            var sink = await SpeechRecognitionAudioSink.InitializeAsync(format, speechModel, fileSink).ConfigureAwait(false);

            sink.OnSpeechRecognized.Subscribe((sender, args) => System.Console.Write(args.Value));

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
