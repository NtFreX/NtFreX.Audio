using NtFreX.Audio.Extensions;
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
            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultRenderDevice();

            (var context, var client) = await device.PlayAsync(audio, cancellationToken).ConfigureAwait(false);

            var totalLength = audio.GetLength().TotalSeconds;
            context.PositionChanged.Subscribe((sender, args) => ConsoleProgressBar.LogProgress(args.Value / totalLength));

            await context.EndOfPositionReached.WaitForNextEvent().ConfigureAwait(false);

            context.Dispose();
            client.Dispose();

            System.Console.WriteLine();
            System.Console.WriteLine("  Audio device has been disposed");
        }
    }
}
