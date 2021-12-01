using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal class RenderAudioDemo : IDemo
    {
        public string Name => nameof(RenderAudioDemo);
        public string Description => "Plays the given audio file";

        private const int InputLoopDelay = 100;
        private const float SeekSkipInSeconds = 0.5f;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"left arrow  => skip {SeekSkipInSeconds} seconds to the past");
            System.Console.WriteLine($"right arrow => skip {SeekSkipInSeconds} seconds to the future");
            System.Console.WriteLine($"space       => pause/continue playing");
            System.Console.WriteLine();

            System.Console.Write("Enter the file you want to play: ");
            var file = System.Console.ReadLine();
            _ = file ?? throw new Exception("Enter a valid file name");

            await using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);

            using var device = AudioDevice.GetDefaultRenderDevice();

            System.Console.WriteLine($"Playing on {device.GetId()}...");
            await using var context = await device.RenderAsync(audio, cancellationToken).ConfigureAwait(false);

            System.Console.WriteLine($"  Length = {context.GetLength()}");
            var format = context.GetFormat();
            AudioFactory.PrintAudioFormat(format);

            //TODO: the given audio will be converted to a format compatible with the used device, this causes the length to be different. Is this expected?
            // audio.GetLength().TotalSeconds != context.GetLength().TotalSeconds
            // this could be because a direct conversion from 44100 bps to 48000 bps leads to presicion loss
            var totalLength = context.GetLength().TotalSeconds;
            context.PositionChanged.Subscribe((sender, args) => ConsoleProgressBar.LogProgress(args.Value / totalLength));
            context.RenderExceptionOccured.Subscribe((sender, args) => System.Console.WriteLine($"  Error: {args.Value.Message}"));

            using var keyboardListenerCancelSource = new CancellationTokenSource();
            using var messageLoopTask = Task.Run(() => KeyboardMessageLoopAsync(context, keyboardListenerCancelSource.Token), keyboardListenerCancelSource.Token);

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
                await messageLoopTask.IgnoreCancelationError().ConfigureAwait(false);

                System.Console.WriteLine();
                System.Console.WriteLine("  The audio playback ended");
            }
        }

        private static async Task KeyboardMessageLoopAsync(IRenderContext renderContext, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if(!System.Console.KeyAvailable)
                {
                    await Task.Delay(InputLoopDelay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                var input = System.Console.ReadKey(true);
                if (input.Key == ConsoleKey.LeftArrow)
                {
                    renderContext.SetPosition(renderContext.GetPosition() - TimeSpan.FromSeconds(0.5));
                }
                else if (input.Key == ConsoleKey.RightArrow)
                {
                    renderContext.SetPosition(renderContext.GetPosition() + TimeSpan.FromSeconds(0.5));
                }
                else if (input.Key == ConsoleKey.Spacebar)
                {
                    if (renderContext.IsStopped())
                    {
                        renderContext.Start();
                    }
                    else
                    {
                        renderContext.Stop();
                    }
                }
            }
        }
    }
}
