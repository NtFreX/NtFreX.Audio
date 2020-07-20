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

            var toPlay = audio
                .AsEnumerable(cancellationToken)
                .LogProgress(ConsoleProgressBar.LogProgress, cancellationToken);

            (var context, var client) = await device.PlayAsync(toPlay, cancellationToken).ConfigureAwait(false);

            await context.EndOfDataReached.WaitForNextEvent().ConfigureAwait(false);

            context.Dispose();
            client.Dispose();

            System.Console.WriteLine();
            System.Console.WriteLine("  Audio device has been disposed");
        }
    }
}
