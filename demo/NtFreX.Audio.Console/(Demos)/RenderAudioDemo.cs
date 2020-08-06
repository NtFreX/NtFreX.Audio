using NtFreX.Audio.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    internal class RenderAudioDemo : IDemo
    {
        public string Name => nameof(RenderAudioDemo);
        public string Description => "Plays the given audio file";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to play: ");
            var file = System.Console.ReadLine();

            using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);

            System.Console.WriteLine($"Playing...");
            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultRenderDevice();

            (var context, var client) = await device.RenderAsync(audio, cancellationToken).ConfigureAwait(false);

            var totalLength = audio.GetLength().TotalSeconds;
            context.PositionChanged.Subscribe((sender, args) => ConsoleProgressBar.LogProgress(args.Value / totalLength));

            try
            {
                await context.EndOfPositionReached.WaitForNextEvent(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                await context.DisposeAsync().ConfigureAwait(false);
                client.Dispose();

                System.Console.WriteLine();
                System.Console.WriteLine("  Audio device has been disposed");
            }
        }
    }
}
