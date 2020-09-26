using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    public class SinGeneratorDemo : IDemo
    {
        public string Name => nameof(SinGeneratorDemo);

        public string Description => "Generates a sin tone with a given length and frequency and saves it to a given file";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to create: ");
            var file = System.Console.ReadLine();

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            System.Console.Write("Enter the frequency: ");
            var frequency = int.Parse(System.Console.ReadLine(), NumberFormatInfo.InvariantInfo);

            System.Console.Write("Enter the length: ");
            var length = int.Parse(System.Console.ReadLine(), NumberFormatInfo.InvariantInfo);

            uint sampleRate = 44100;
            var format = new AudioFormat(sampleRate, 64, 1, AudioFormatType.IeeFloat);
            var data = WaveBuilder.Sin(sampleRate, frequency, length);
            await using var audio = await IntermediateAudioContainerBuilder
                .Build(format, data, sampleRate * format.BitsPerSample * format.Channels * length)
                .ToFileAsync(file, FileMode.CreateNew, cancellationToken).ConfigureAwait(false);
        }
    }
}
