using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using NtFreX.Audio.Samplers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{

    internal class SampleAudioSample : ISample
    {
        public string Name => nameof(SampleAudioSample);
        public string Description => "Converts the given audio file and saves it to the given path";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to convert: ");
            var file = System.Console.ReadLine();

            System.Console.Write("Enter the path where you want to save your converted file to: ");
            var target = System.Console.ReadLine();

            using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);

            System.Console.WriteLine($"Converting...");
            if (File.Exists(target))
            {
                File.Delete(target);
            }

            // TODO: make pipe configurable through console input
            using var convertedAudio = await new AudioSamplerPipe()
                //.Add(x => x.MonoAudioSampler())
                .Add(x => x.MonoToStereoAudioSampler())
                .Add(x => x.SpeedAudioSampler(4))
                .Add(x => x.SampleRateAudioSampler(44100))
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
                //.Add(x => x.VolumeAudioSampler(8))
                .RunAsync(audio.AsEnumerable(cancellationToken), cancellationToken)
                .LogProgress(ConsoleHelper.LogProgress, cancellationToken)
                .ToFileAsync(target, FileMode.OpenOrCreate, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            System.Console.WriteLine();
        }
    }
}
