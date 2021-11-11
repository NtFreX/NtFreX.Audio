using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    public sealed class Program
    {
        private static string[] SampleAudios => new string[] 
        {
            @"..\..\..\..\..\resources\audio\8-bit Detective.wav",
            @"..\..\..\..\..\resources\audio\Dash Runner.wav",
            @"..\..\..\..\..\resources\audio\1000hz_sinwave.wav"
        };

        //TODO: converter demo
        private static IDemo[] Demos => new IDemo[]
        {
            new RenderAudioDemo(),
            new CaptureAudioDemo(),
            new SampleAudioDemo(),
            new SinGeneratorDemo(),
            new GetDefaulRenderDeviceDemo(),
            new DrawDiagramsDemo()
        };

        private const string ExitKey = "x";

        public static async Task Main()
        {
            var cancellationTokenSources = new Stack<CancellationTokenSource>();

            System.Console.ForegroundColor = ConsoleColor.Green;
            PrintTitle();

            System.Console.ForegroundColor = ConsoleColor.White;
            PrintInstructions();

            System.Console.ForegroundColor = ConsoleColor.Gray;
            PrintSampleAudioFiles();

            System.Console.WriteLine("Using dotnet v" + Environment.Version);
            System.Console.WriteLine();

            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) =>
            {
                if(cancellationTokenSources.TryPeek(out var tokenSource))
                {
                    tokenSource.Cancel();
                }
                e.Cancel = true;
            };

            while (true)
            {
                PrintDemos();

                var input = System.Console.ReadLine();
                if (input == ExitKey)
                {
                    break;
                }

                if (!int.TryParse(input, out var number) || number <= 0 || number > Demos.Length)
                {
                    System.Console.WriteLine("Invalid input");
                }
                else
                {
                    try
                    {
#pragma warning disable CA2000 // Dispose objects before losing scope
                        cancellationTokenSources.Push(new CancellationTokenSource());
#pragma warning restore CA2000 // Dispose objects before losing scope

                        await Demos[number - 1].RunAsync(cancellationTokenSources.Peek().Token).ConfigureAwait(false);
                    }
                    catch (Exception exce)
                    {
                        LogException(exce);
                    }
                }
            }

            foreach(var cancelationTokenSource in cancellationTokenSources)
            {
                cancelationTokenSource.Dispose();
            }
        }

        private static void PrintDemos()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("==================================================================================================================");
            System.Console.WriteLine("Choose the demo you want to run:");
            for (var i = 0; i < Demos.Length; i++)
            {
                System.Console.WriteLine($"  {i + 1} - {Demos[i].Name} ({Demos[i].Description})");
            }
            System.Console.WriteLine($"  {ExitKey} - Quit application");
        }

        private static void PrintSampleAudioFiles()
        {
            System.Console.WriteLine("Test audio files:");
            foreach (var file in SampleAudios)
            {
                System.Console.WriteLine($"  - {file}");
            }
            System.Console.WriteLine();
        }

        private static void PrintTitle()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(@" ________   _________  ________ ________  _______      ___    ___ ________  ___  ___  ________  ___  ________     ");
            System.Console.WriteLine(@"|\   ___  \|\___   ___\\  _____\\   __  \|\  ___ \    |\  \  /  /|\   __  \|\  \|\  \|\   ___ \|\  \|\   __  \    ");
            System.Console.WriteLine(@"\ \  \\ \  \|___ \  \_\ \  \__/\ \  \|\  \ \   __/|   \ \  \/  / | \  \|\  \ \  \\\  \ \  \_|\ \ \  \ \  \|\  \   ");
            System.Console.WriteLine(@" \ \  \\ \  \   \ \  \ \ \   __\\ \   _  _\ \  \_|/__  \ \    / / \ \   __  \ \  \\\  \ \  \ \\ \ \  \ \  \\\  \  ");
            System.Console.WriteLine(@"  \ \  \\ \  \   \ \  \ \ \  \_| \ \  \\  \\ \  \_|\ \  /     \/ __\ \  \ \  \ \  \\\  \ \  \_\\ \ \  \ \  \\\  \ ");
            System.Console.WriteLine(@"   \ \__\\ \__\   \ \__\ \ \__\   \ \__\\ _\\ \_______\/  /\   \|\__\ \__\ \__\ \_______\ \_______\ \__\ \_______\");
            System.Console.WriteLine(@"    \|__| \|__|    \|__|  \|__|    \|__|\|__|\|_______/__/ /\ __\|__|\|__|\|__|\|_______|\|_______|\|__|\|_______|");
            System.Console.WriteLine(@"                                                      |__|/ \|__|                                                 ");
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        private static void PrintInstructions()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("<ctrl> + c => cancel current action");
            System.Console.WriteLine();
        }

        private static void LogException(Exception exce, int tabs = 0)
        {
            var spacing = string.Join(string.Empty, Enumerable.Repeat("  ", tabs));

            System.Console.WriteLine();
            System.Console.WriteLine(spacing + exce.Message);

            if(exce is COMException comException)
            {
                System.Console.WriteLine($"{spacing}HResult: {comException.HResult}");
            }

            System.Console.WriteLine(spacing + exce.StackTrace);

            if(exce.InnerException != null)
            {
                LogException(exce.InnerException, ++tabs);
            }
        }
    }
}
