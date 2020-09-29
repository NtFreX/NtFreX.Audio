using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Extensions;
using System;
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
            _ = file ?? throw new Exception("Enter a valid file name");

            await using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);

            var audioPlatform = AudioEnvironment.Platform.Get();
            using var device = audioPlatform.AudioDeviceFactory.GetDefaultRenderDevice();

            System.Console.WriteLine($"Playing on {device.GetId()}...");
            await using var context = await device.RenderAsync(audio, cancellationToken).ConfigureAwait(false);

            var format = context.GetFormat();
            AudioFactory.PrintAudioFormat(format);

            var totalLength = audio.GetLength().TotalSeconds;
            context.PositionChanged.Subscribe((sender, args) => ConsoleProgressBar.LogProgress(args.Value / totalLength));
            context.RenderExceptionOccured.Subscribe((sender, args) => System.Console.WriteLine($"  Error: {args.Value.Message}"));

            using var keyboardListenerCancelSource = new CancellationTokenSource();
            _ = Task.Run(() => KeyboardMessageLoop(context, keyboardListenerCancelSource.Token), keyboardListenerCancelSource.Token).ConfigureAwait(false);

            try
            {
                await Task.WhenAny(
                    context.EndOfPositionReached.NextEvent(cancellationToken),
                    context.RenderExceptionOccured.NextEvent(cancellationToken))
                    .ConfigureAwait(false);
            }
            finally
            {
                keyboardListenerCancelSource.Cancel();

                System.Console.WriteLine();
                System.Console.WriteLine("  The end of the audio was reached or a render exception occurred or render was canceled");
            }
        }

        private static void KeyboardMessageLoop(IRenderContext renderContext, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var input = System.Console.ReadKey(true);
                if (input.Key == ConsoleKey.LeftArrow)
                {
                    renderContext.SetPosition(renderContext.GetPosition() - TimeSpan.FromSeconds(0.5));
                }
                else if (input.Key == ConsoleKey.RightArrow)
                {
                    renderContext.SetPosition(renderContext.GetPosition() + TimeSpan.FromSeconds(0.5));
                }
            }
        }
    }
}
