using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal class DrawDiagramsDemo : IDemo
    {
        public string Name => nameof(DrawDiagramsDemo);
        public string Description => "Draws a diagram of the given audio file and saves it as svg to an given html file";

        private const int SvgHeight = 1000;
        private const float SvgMiddle = SvgHeight / 2f;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to draw: ");
            var file = System.Console.ReadLine();

            System.Console.Write("Enter the file you write to: ");
            var target = System.Console.ReadLine();

            using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);

            System.Console.WriteLine($"Drawing...");
            File.WriteAllText(target, Html(await DrawSampleWavesAsync(audio).ConfigureAwait(false), DrawAudioControls(file)/*, await DrawSectogramAsync(audio).ConfigureAwait(false)*/));
        }
        private static async Task<IEnumerable<Sample[]>> GetChannelAudioSamplesAsync(WaveStreamAudioContainer waveAudioContainer)
        {
            var channels = new List<Sample>[waveAudioContainer.FmtSubChunk.Channels];
            var currentChannel = 0;
            await foreach (var value in waveAudioContainer.GetAudioSamplesAsync().ConfigureAwait(false))
            {
                if (channels[currentChannel] == null)
                {
                    channels[currentChannel] = new List<Sample>();
                }

                channels[currentChannel].Add(value);

                if (++currentChannel >= waveAudioContainer.FmtSubChunk.Channels)
                {
                    currentChannel = 0;
                }
            }
            return channels.Select(x => x.ToArray());
        }

        private static string DrawAudioControls(string path)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<audio controls>");
            html.AppendLine($"<source src=\"{path}\" type=\"audio/wav\">");
            html.AppendLine("</audio>");
            return html.ToString();
        }

        private static async Task<string> DrawSampleWavesAsync(WaveStreamAudioContainer waveAudioContainer)
        {
            var channelSamples = await GetChannelAudioSamplesAsync(waveAudioContainer).ConfigureAwait(false);

            return DrawSvg(
                data: channelSamples.ToArray(), 
                colors: new[] { "green", "red", "yellow", "blue", "deeppink", "indigo", "gray", "cyan", "coral", "black" },
                opacities: Enumerable.Repeat(1f, 10).ToArray(),
                format: waveAudioContainer.Format);
        }

        private static string Html(params string[] elements)
        {
            var html = new StringBuilder();
            html.AppendLine("<html><head></head><body>");
            foreach (var element in elements)
            {
                html.AppendLine(element);
            }
            html.AppendLine("</body></html>");
            return html.ToString();
        }

        private static string DrawSvg(Sample[][] data, string[] colors, float[] opacities, IAudioFormat format)
        {
            var width = data[0].Length;
            var maxValue = data.SelectMany(x => x).Max(x => x.Value);
            var modifier = SvgHeight / (maxValue * 2f);

            var image = new StringBuilder();
            image.AppendLine($"<svg height=\"{SvgHeight}\" width=\"{width}\" style=\"border: 1px solid black;\">");
            for (int i = 0; i < data.Length; i++)
            {
                image.AppendLine(DrawPath(data[i], colors[i], opacities[i], modifier));
            }

            var textSpacing = SvgHeight / 50f;
            var topDescription = SvgHeight / 10f;
            var leftDescription = 20;
            image.AppendLine($"<text x=\"{leftDescription}\" y=\"{topDescription + (textSpacing * 1)}\">SampleRate: {format.SampleRate}</text>");
            image.AppendLine($"<text x=\"{leftDescription}\" y=\"{topDescription + (textSpacing * 2)}\">BitsPerSample: {format.BitsPerSample}</text>");
            image.AppendLine($"<text x=\"{leftDescription}\" y=\"{topDescription + (textSpacing * 3)}\">Channels: {format.Channels}</text>");
            image.AppendLine($"<text x=\"{leftDescription}\" y=\"{topDescription + (textSpacing * 4)}\">Type: {format.Type}</text>");

            image.AppendLine($"<text x=\"{leftDescription}\" y=\"{textSpacing}\">max {maxValue}</text>");
            image.AppendLine($"<line x1=\"0\" y1=\"{SvgMiddle}\" x2=\"{width}\" y2=\"{SvgMiddle}\" style=\"stroke: rgb(0, 0, 0); stroke-width:1;\" />");
            image.AppendLine($"<text x=\"{leftDescription}\" y=\"{SvgHeight - textSpacing}\">min {-maxValue}</text>");

            for (int i = 0; i < width; i += (int)(format.SampleRate / 100f))
            {
                var second = i / (format.SampleRate * 1f);

                image.AppendLine($"<text x=\"{i}\" y=\"{SvgMiddle}\">{second}s</text>");
                image.AppendLine($"<line x1=\"{i}\" y1=\"{0}\" x2=\"{i}\" y2=\"{SvgHeight}\" style=\"stroke: rgb(0, 0, 0); stroke-width:1;\" />");
            }

            image.AppendLine("</svg>");
            return image.ToString();
        }

        private static string DrawPath(Sample[] data, string color, float opacity, double modifier)
        {
            var path = new StringBuilder();

            path.AppendLine($"<path stroke=\"{color}\" stroke-width=\"1\" stroke-opacity=\"{opacity}\" fill-opacity=\"0\" d=\"M0 {SvgMiddle}");
            for (int i = 0; i < data.Length; i++)
            {
                var value = data[i].Value * modifier;
                path.AppendLine($"L{i} {SvgMiddle + value} ");
            }
            path.AppendLine("\" />");
            return path.ToString();
        }

        /*
        static async Task<string> DrawSectogramAsync(WaveStreamAudioContainer waveAudioContainer)
        {
            // TODO: cleanup improve make it work
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
        */
    }
}
