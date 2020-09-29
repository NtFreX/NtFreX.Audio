using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using NtFreX.Audio.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
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
            _ = file ?? throw new Exception("Enter a valid file name");

            System.Console.Write("Enter the file you write to: ");
            var target = System.Console.ReadLine();
            _ = target ?? throw new Exception("Enter a valid file name");

            await using var audio = await AudioFactory
                .GetSampleAudioAsync(file, cancellationToken)
                .ConvertAsync<IntermediateEnumerableAudioContainer>(cancellationToken)
                .ConfigureAwait(false);

            System.Console.WriteLine($"Drawing...");
            var html = Html(
                await DrawSampleWavesAsync(audio).ConfigureAwait(false),
                DrawAudioControls(file),
                await DrawSectogramAsync(audio).ConfigureAwait(false));
            File.WriteAllText(target, html);
        }

        private static async Task<IEnumerable<Sample[]>> GetChannelAudioSamplesAsync(IntermediateAudioContainer container)
        {
            var format = container.GetFormat();
            var channels = new List<Sample>[format.Channels];
            var currentChannel = 0;
            await foreach(var value in container)
            {
                if (channels[currentChannel] == null)
                {
                    channels[currentChannel] = new List<Sample>();
                }

                channels[currentChannel].Add(value);

                if (++currentChannel >= format.Channels)
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

        private static async Task<string> DrawSampleWavesAsync(IntermediateEnumerableAudioContainer waveAudioContainer)
        {
            var channelSamples = await GetChannelAudioSamplesAsync(waveAudioContainer).ConfigureAwait(false);

            return DrawSvg(
                data: channelSamples.ToArray(), 
                colors: new[] { "green", "red", "yellow", "blue", "deeppink", "indigo", "gray", "cyan", "coral", "black" },
                opacities: Enumerable.Repeat(1f, 10).ToArray(),
                format: waveAudioContainer.GetFormat());
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

        private static async Task<string> DrawSectogramAsync(IntermediateEnumerableAudioContainer waveAudioContainer)
        {
            var stepSize = 200;
            
            using var monoAudio = await AudioEnvironment.Sampler
                .ChannelAudioSampler(1)
                .SampleAsync(waveAudioContainer)
                .ToInMemoryContainerAsync()
                .ConfigureAwait(false);

            var monoData = await monoAudio.SelectAsync(x => new Complex(x.Value, 0)).ToArrayAsync().ConfigureAwait(false);
            var steps = monoData.Length / stepSize;
            var data = new double[steps][];
            for (var col = 0; col < steps; col++)
            {
                var computed = FourierTransform
                    .Fast(monoData.Skip(stepSize * col).Take(stepSize).ToArray())
                    .ToArray();

                data[col] = new double[computed.Length];
                for (int row = 0; row < computed.Length; row++)
                {
                    data[col][row] = computed[row].Magnitude;
                }
            }

            var max = data.SelectMany(x => x).Max();
            using var bitmap = new Bitmap(data.Length, data[0].Length, PixelFormat.Format32bppArgb);
            for (var col = 0; col < bitmap.Width; col++)
            {
                for (int row = 0; row < bitmap.Height; row++)
                {
                    var factor = data[col][row] / max;
                    var value = (int)(factor * byte.MaxValue);

                    bitmap.SetPixel(col, row, Color.FromArgb(0, 0, value));
                }
            }

            return $"<div>{ToBase64Image(bitmap, ImageFormat.Jpeg)}</div>";
        }

        private static string ToBase64String(Bitmap bmp, ImageFormat imageFormat)
        {
            using var memoryStream = new MemoryStream();
            bmp.Save(memoryStream, imageFormat);

            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();

            memoryStream.Close();

            return Convert.ToBase64String(byteBuffer);
        }

        private static string ToBase64Image(Bitmap bmp, ImageFormat imageFormat)
        {
            var base64String = ToBase64String(bmp, imageFormat);
            var img = new StringBuilder();
            img.Append("<img src=\"data: image/" + imageFormat.ToString() + "; base64,");
            img.Append(base64String + "\" ");
            img.Append("width=\"" + bmp.Width + "\" ");
            img.Append("height=\"" + bmp.Height + "\" />");

            return img.ToString();
        }
    }
}
