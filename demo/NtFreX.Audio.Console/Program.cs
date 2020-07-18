using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
using NtFreX.Audio.Samplers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    //TODO: dotnet core 5 and data class
    //TODO: see over disposing and passing of cancelation token and configure await 

    // audio splitting, wave visualization, spectrogram
    // https://towardsdatascience.com/understanding-audio-data-fourier-transform-fft-spectrogram-and-speech-recognition-a4072d228520
    // down/up sampling
    class Program
    {
        const string testWav2 = @"..\..\..\..\..\resources\8-bit Detective.wav";
        const string testWav3 = @"..\..\..\..\..\resources\Dash Runner.wav";
        const string testWav4 = @"..\..\..\..\..\resources\1000hz_sinwave.wav";

        static async Task Main()
        {
            var cancelationTokenSource = new CancellationTokenSource();

            System.Console.WriteLine($"Reading...");
            using var audio = await AudioEnvironment.Serializer.FromFileAsync(testWav2, cancelationTokenSource.Token).ConfigureAwait(false);
            System.Console.WriteLine($"  Length = {audio.GetLength()}");
            
            if (audio is WaveStreamAudioContainer waveAudioContainer)
            {
                System.Console.WriteLine($"Converting...");
                if(File.Exists("mono8bit.wav"))
                {
                    File.Delete("mono8bit.wav");
                }

                using var convertedAudio = await new AudioSamplerPipe()
                    //.Add(x => x.MonoAudioSampler())
                    .Add(x => x.BitsPerSampleAudioSampler(32)).Add(x => x.VolumeAudioSampler(256)) //HINT: changing height of wave a second time helps
                    //.Add(x => x.VolumeAudioSampler(1.0/256)).Add(x => x.BitsPerSampleAudioSampler(16)) //HINT: changing height of wave a second time helps
                    //.Add(x => x.BitsPerSampleAudioSampler(8)) //HINT: changing height of wave a second time helps
                    //.Add(x => x.VolumeAudioSampler(0.5))
                    //.Add(x => x.ShiftWaveAudioSampler(8000))
                    //.Add(x => x.BitsPerSampleAudioSampler(64))
                    //.Add(x => x.BitsPerSampleAudioSampler(32))
                    //.Add(x => x.BitsPerSampleAudioSampler(16))
                    //.Add(x => x.BitsPerSampleAudioSampler(8))
                    //.Add(x => x.ShiftWaveAudioSampler(-64))
                    //.Add(x => x.VolumeAudioSampler(2))
                    .Add(x => x.SampleRateAudioSampler(48000))
                    //.Add(x => x.VolumeAudioSampler(8))
                    .RunAsync(waveAudioContainer, cancelationTokenSource.Token)
                    .LogProgress(LogProgress, cancelationTokenSource.Token)
                    .ToFileAsync("mono8bit.wav", FileMode.OpenOrCreate, cancellationToken: cancelationTokenSource.Token)
                    .ConfigureAwait(false);

                System.Console.WriteLine();
                //System.Console.WriteLine($"Drawing...");
                //File.WriteAllText("waves.html", Html(await DrawSampleWavesAsync(waveAudioContainer, convertedAudio).ConfigureAwait(false), await DrawSectogramAsync(waveAudioContainer).ConfigureAwait(false)));

                System.Console.WriteLine($"Playing...");
                using var device = AudioEnvironment.Device.Get();
                using var context = await device.PlayAsync(
                    await Task
                    .FromResult(WaveEnumerableAudioContainer.ToEnumerable(convertedAudio, cancelationTokenSource.Token))
                    .LogProgress(LogProgress, cancelationTokenSource.Token)
                    .ConfigureAwait(false), cancelationTokenSource.Token).ConfigureAwait(false);

                await context.EndOfDataReached.WaitForNextEvent().ConfigureAwait(false);

                //using var file = await convertedAudio.ToStream("mono8bit.wav", FileMode.OpenOrCreate, cancellationToken: cancelationTokenSource.Token).ConfigureAwait(false);

                System.Console.WriteLine();
                System.Console.WriteLine("  Audio device has been disposed");
            }
        }

        static double lastProgress = 0;
        const int length = 40;
        const int left = 2;
        static void LogProgress(double progress)
        {
            var diff = System.Math.Abs(progress - lastProgress);
            if (diff > 0.01 || progress == 0 || progress == 1)
            {
                System.Console.CursorLeft = left;
                System.Console.Write("<" + string.Join(string.Empty, Enumerable.Repeat("█", (int)(length * progress))));
                System.Console.CursorLeft = length + 1 + left;
                System.Console.Write(">");
                lastProgress = progress;
            }
        }

        static async Task<string> DrawSectogramAsync(WaveStreamAudioContainer waveAudioContainer)
        {
            using var monoAudio = await AudioEnvironment.Sampler
                .MonoAudioSampler()
                .SampleAsync(waveAudioContainer)
                .ToInMemoryContainerAsync()
                .ConfigureAwait(false);

            //using var downAudio = await AudioEnvironment.Sampler.BitsPerSampleAudioSampler(8).SampleAsync(monoAudio).ConfigureAwait(false);
            var monoData = await monoAudio.GetAudioSamplesAsync().SelectAsync(x => x.ToInt64()).ToArrayAsync().ConfigureAwait(false);

            var spectrum = new StringBuilder();
            var computed = FourierTransform
                .Fast(monoData.Select(x => new CartesianCordinate(x)).ToArray())
                .Where(x => x != null)
                .ToArray();

            var height = computed.Max(x => x.X) + computed.Max(x => x.Y);
            var halfY = height / 2.0f;

            var skip = 50;
            spectrum.AppendLine($"<svg viewBox=\"0 0 {computed.Length} {height}\" height=\"40%\" width=\"40%\">"); // viewBox=\"0 0 {width} {height}\" height=\"100%\" width=\"100%\" style=\"position:absolute;z-index:1;\" 
            spectrum.AppendLine($"<path stroke=\"black\" stroke-width=\"{2}\" stroke-opacity=\"1\" fill-opacity=\"0\" d=\"M0 {halfY}"); // height=\"{height}\" width=\"{width}\" 
            for (int i = 0; i < computed.Length; i += skip)
            {
                var current = computed.Skip(i).Take(skip).ToArray();
                var averageX = current.Average(x => x.X);
                var averageY = current.Average(x => x.Y);
                spectrum.AppendLine($"L{i} {(averageX + averageY) / 100.0} ");
            }
            spectrum.AppendLine("Z\" />");
            spectrum.AppendLine("</svg>");
            return spectrum.ToString();
        }

        static async Task<string> DrawSampleWavesAsync(WaveStreamAudioContainer waveAudioContainer, WaveStreamAudioContainer converted)
        {
            var convertedData = await converted.GetAudioSamplesAsync().SelectAsync(x => x.ToInt64()).ToArrayAsync().ConfigureAwait(false);
            var channelSamples = await GetChannelAudioSamplesAsync(waveAudioContainer).ConfigureAwait(false);

            var data = new List<long[]>(channelSamples)
            {
                convertedData
            };
            return DrawSvg(data.ToArray(), new[] { "green", "red", "black" }, new[] { 0.2f, 0.2f, 1 });
        }

        static async Task<IEnumerable<long[]>> GetChannelAudioSamplesAsync(WaveStreamAudioContainer waveAudioContainer)
        {
            var channels = new List<long>[waveAudioContainer.FmtSubChunk.NumChannels];
            var currentChannel = 0;
            await foreach (var value in waveAudioContainer.GetAudioSamplesAsync().ConfigureAwait(false))
            {
                if (channels[currentChannel] == null)
                {
                    channels[currentChannel] = new List<long>();
                }

                channels[currentChannel].Add(value.ToInt64());

                if (++currentChannel >= waveAudioContainer.FmtSubChunk.NumChannels)
                {
                    currentChannel = 0;
                }
            }
            return channels.Select(x => x.ToArray());
        }

        static string Html(params string[] elements)
        {
            var html = new StringBuilder();
            html.AppendLine("<html><head></head><body>");
            foreach(var element in elements)
            {
                html.AppendLine(element);
            }
            html.AppendLine("</body></html>");
            return html.ToString();
        }

        const float xModifier = 0.01f;
        const float yModifier = 0.05f;

        static string DrawSvg(long[][] data, string[] colors, float[] opacities, int skip = 100)
        {
            var width = data[0].Length * xModifier; // TODO: replace with file length
            var height = data[0].Max() * yModifier; // TODO: replace max with waveAudioContainer.FmtSubChunk.BitsPerSample
            var middle = height / 2.0f;

            var image = new StringBuilder();
            image.AppendLine($"<svg viewBox=\"0 0 {width} {height}\" height=\"100%\" width=\"500%\">"); //  style=\"position:absolute;z-index:2;\"
            image.AppendLine($"<line x1=\"0\" y1=\"{middle}\" x2=\"{width}\" y2=\"{middle}\" style=\"stroke: rgb(0, 0, 0); stroke-width:1;\" />");
            for (int i = 0; i < data.Length; i++)
            {
                image.AppendLine(DrawPath(data[i], colors[i], opacities[i], height, skip));
            }
            image.AppendLine("</svg>");
            return image.ToString();
        }

        static string DrawPath(long[] data, string color, float opacity, float height, int skip)
        {
            var middle = height / 2.0f;

            var path = new StringBuilder();
            path.AppendLine($"<path stroke=\"{color}\" stroke-width=\"1\" stroke-opacity=\"{opacity}\" fill-opacity=\"0\" d=\"M0 {middle}");
            for (int i = 0; i < data.Length; i += skip)
            {
                var averageData = data.Skip(i).Take(skip).Average();
                path.AppendLine($"L{i * xModifier} {middle - (averageData * yModifier)} ");
            }
            path.AppendLine("\" />");
            return path.ToString();
        }
    }

}
