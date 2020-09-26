using NtFreX.Audio.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal class RenderAudioDemo : IDemo
    {
        public string Name => nameof(RenderAudioDemo);
        public string Description => "Plays the given audio file";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to play: ");
            var file = System.Console.ReadLine();

            await using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);

            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultRenderDevice();

            System.Console.WriteLine($"Playing on {device.GetId()}...");
            await using var context = await device.RenderAsync(audio, cancellationToken).ConfigureAwait(false);

            var format = context.GetFormat();
            AudioFactory.PrintAudioFormat(format);

            var totalLength = audio.GetLength().TotalSeconds;
            context.PositionChanged.Subscribe((sender, args) => ConsoleProgressBar.LogProgress(args.Value / totalLength));

            try
            {
                await context.EndOfPositionReached.WaitForNextEvent(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                System.Console.WriteLine();
                System.Console.WriteLine("  Audio device has been disposed");
            }
        }
    }
}
