using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    //TODO: dotnet core 5 and data class
    //TODO: see over disposing and passing of cancelation token and configure await 

    // audio splitting, wave visualization, spectrogram
    // https://towardsdatascience.com/understanding-audio-data-fourier-transform-fft-spectrogram-and-speech-recognition-a4072d228520
    class Program
    {
        static readonly string[] sampleAudios = new [] 
        {
            @"..\..\..\..\..\resources\8-bit Detective.wav",
            @"..\..\..\..\..\resources\Dash Runner.wav",
            @"..\..\..\..\..\resources\1000hz_sinwave.wav"
        };

        static readonly IDemo[] demos = new IDemo[]
        {
            new PlayAudioDemo(),
            new SampleAudioDemo(),
            new DrawDiagramsDemo()
        };

        static async Task Main()
        {
            using var cancellationTokenSource = new CancellationTokenSource();

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

            System.Console.WriteLine("Test audio files:");
            foreach(var file in sampleAudios)
            {
                System.Console.WriteLine($"  - {file}");
            }
            System.Console.WriteLine();

            System.Console.CancelKeyPress += (object sender, System.ConsoleCancelEventArgs e) =>
            {
                cancellationTokenSource.Cancel();
                System.Environment.Exit(0);
            };

            const string exitKey = "x";
            while (true)
            {
                System.Console.WriteLine("Choose the demo you want to run");
                for (var i = 0; i < demos.Length; i++)
                {
                    System.Console.WriteLine($"  {i + 1} - {demos[i].Name} ({demos[i].Description})");
                }
                System.Console.WriteLine($"  {exitKey} - Quit application");

                var input = cancellationTokenSource.IsCancellationRequested ? exitKey : System.Console.ReadLine();
                if (input == exitKey)
                {
                    cancellationTokenSource.Cancel();
                    break;
                }

                if (!int.TryParse(input, out var number) || number <= 0 || number > demos.Length)
                {
                    System.Console.WriteLine("Invalid input");
                }
                else
                {
                    try
                    {
                        await demos[number - 1].RunAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    catch (Exception exce)
                    {
                        System.Console.WriteLine(exce.Message);
                    }
                }
            }
        }
    }
}
