﻿using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
using NtFreX.Audio.Samplers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    // audio splitting, wave visualization, spectrogram
    // https://towardsdatascience.com/understanding-audio-data-fourier-transform-fft-spectrogram-and-speech-recognition-a4072d228520
    // down/up sampling
    class Program
    {
        const string testWav = @"E:\Programs\Steam\steamapps\common\The Beginners Guide\beginnersguide\sound\narration\VOF\VOF_machine08.wav";
        const string testWav2 = @"C:\Users\FTR\Downloads\8-bit Detective.wav";

        static async Task Main(string[] args)
        {
            var serializer = AudioEnvironment.Serializer;
            var audio = await serializer.FromFileAsync(testWav2).ConfigureAwait(false);

            System.Console.WriteLine($"Length = {audio.GetLength()}");
            
            if (audio is WaveAudioContainer waveAudioContainer)
            {
                await AudioEnvironment.Serializer.ToFileAsync("mono.wav", await waveAudioContainer.SampleToMonoAsync().ConfigureAwait(false));

                File.WriteAllText("waves.html", Html(
                    await DrawSampleWavesAsync(waveAudioContainer).ConfigureAwait(false)/*,
                    spectrum.ToString()*/));
            }

            using (var device = AudioEnvironment.Device.Get()) 
            {
                await await device.PlayAsync(audio);
            }

            System.Console.WriteLine("Audio device has been disposed");
            System.Console.ReadKey();
        }

        static void DrawSectogram()
        {

            /*
            var spectrum = new StringBuilder();
            var count = interleavedChannelData.Length / 2;
            for (var c = 0; c < interleavedChannelData.Length / count - 1; c++)
            {
                var computed = FourierTransform
                    .Fast(interleavedChannelData.Skip(c * count).Take(count).Select(x => new CartesianCordinate(x)).ToArray())
                    .Where(x => x != null)
                    .ToArray();
                var width = computed.Max(x => x.X);
                var height = computed.Max(x => x.Y);
                var max = System.Math.Max(width, height);
                var halfX = width / 2.0f;
                var halfY = height / 2.0f;

                var skip = 50;
                spectrum.AppendLine($"<svg viewBox=\"0 0 {width} {height}\" height=\"40%\" width=\"40%\">"); // viewBox=\"0 0 {width} {height}\" height=\"100%\" width=\"100%\" style=\"position:absolute;z-index:1;\" 
                spectrum.AppendLine($"<path stroke=\"black\" stroke-width=\"{max / 400f}\" stroke-opacity=\"1\" fill-opacity=\"0\" d=\"M{halfX} {halfY}"); // height=\"{height}\" width=\"{width}\" 
                for (int i = 0; i < computed.Length; i += skip)
                {
                    var current = computed.Skip(i).Take(skip).ToArray();
                    var averageX = current.Average(x => x.X);
                    var averageY = current.Average(x => x.Y);
                    spectrum.AppendLine($"L{averageX + halfX} {averageY + halfY} ");
                }
                spectrum.AppendLine("Z\" />");
                spectrum.AppendLine("</svg>");
            }
            */
        }

        static async Task<string> DrawSampleWavesAsync(WaveAudioContainer waveAudioContainer)
        {
            var samples = waveAudioContainer.GetAudioSamplesAsync();
            var interleavedChannelData = await MonoAudioSampler.InterleaveChannelDataAsync(samples, waveAudioContainer.FmtSubChunk.NumChannels).ConfigureAwait(false);
            var channelSamples = await GetChannelAudioSamplesAsync(waveAudioContainer).ConfigureAwait(false);
            
            var data = new List<int[]>(channelSamples);
            data.Add(interleavedChannelData);
            return DrawSvg(data.ToArray(), new[] { "green", "red", "black" }, new[] { 0.4f, 0.4f, 1 });
        }

        static async Task<IEnumerable<int[]>> GetChannelAudioSamplesAsync(WaveAudioContainer waveAudioContainer)
        {
            var channels = new List<int>[waveAudioContainer.FmtSubChunk.NumChannels];
            var currentChannel = 0;
            await foreach (var value in waveAudioContainer.GetAudioSamplesAsync().ConfigureAwait(false))
            {
                if (channels[currentChannel] == null)
                {
                    channels[currentChannel] = new List<int>();
                }

                channels[currentChannel].Add(value.ToInt32());

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

        static string DrawSvg(int[][] data, string[] colors, float[] opacities, int skip = 100)
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

        static string DrawPath(int[] data, string color, float opacity, float height, int skip)
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
