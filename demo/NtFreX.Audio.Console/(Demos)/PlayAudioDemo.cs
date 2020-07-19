using NtFreX.Audio.Extensions;
using NtFreX.Audio.Samplers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    internal class PlayAudioDemo : IDemo
    {
        public string Name => nameof(PlayAudioDemo);
        public string Description => "Plays the given audio file";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to play: ");
            var file = System.Console.ReadLine();

            using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);

            System.Console.WriteLine($"Playing...");
            using var device = AudioEnvironment.Device.Get();

            var toPlay = audio
                .AsEnumerable(cancellationToken)
                .LogProgress(ConsoleProgressBar.LogProgress, cancellationToken);

            //TODO: create api to initialize and convert if possible
            if (!device.TryInitialize(toPlay, out var supportedFormat))
            {
                // TODO convert everyting nessesary
                toPlay = await new AudioSamplerPipe()
                    .Add(x => x.BitsPerSampleAudioSampler(supportedFormat.BitsPerSample))
                    .Add(x => x.SampleRateAudioSampler(supportedFormat.SampleRate))
                    .RunAsync(toPlay, cancellationToken)
                    .ConfigureAwait(false);

                if (!device.TryInitialize(toPlay, out _))
                {
                    throw new Exception("Not supported");
                }
            }

            using var context = await device.PlayAsync(cancellationToken).ConfigureAwait(false);

            await context.EndOfDataReached.WaitForNextEvent().ConfigureAwait(false);

            System.Console.WriteLine();
            System.Console.WriteLine("  Audio device has been disposed");
        }
    }
}
