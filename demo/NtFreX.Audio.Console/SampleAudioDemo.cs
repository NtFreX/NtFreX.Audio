using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using NtFreX.Audio.Samplers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{

    internal class SampleAudioDemo : IDemo
    {
        public string Name => nameof(SampleAudioDemo);
        public string Description => "Converts the given audio file and saves it to the given path";

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            System.Console.Write("Enter the file you want to convert: ");
            var file = System.Console.ReadLine();

            System.Console.Write("Enter the path where you want to save your converted file to: ");
            var target = System.Console.ReadLine();

            System.Console.WriteLine("Configure your pipeline");
            var pipe = new AudioSamplerPipe();
            var runCommand = "run";
            while (true)
            {
                var samplers = GetAudioSamplerNames();
                for (var i = 0; i < samplers.Length; i++)
                {
                    System.Console.WriteLine($"  {i + 1} - {GetDisplayName(samplers[i])}");
                }
                System.Console.WriteLine($"  {runCommand} - runs the pipeline");

                var input = System.Console.ReadLine();

                if(input == runCommand)
                {
                    break;
                }

                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if(!int.TryParse(parts[0], out var number) || number <= 0 || number > samplers.Length)
                {
                    System.Console.WriteLine("Invalid input");
                }
                else
                {
                    try
                    {
                        //TODO: convert to correct parameter type
                        var args = parts.Skip(1).Select(x => x.Contains('.') ? (object)double.Parse(x) : (object)uint.Parse(x)).ToArray();
                        pipe.Add(x => (AudioSampler)typeof(AudioSamplerFactory).GetMethod(samplers[number - 1]).Invoke(x, args));
                    }
                    catch (Exception exce)
                    {
                        System.Console.WriteLine(exce.Message);
                    }
                }
            }

            using var audio = await AudioFactory.GetSampleAudioAsync(file, cancellationToken).ConfigureAwait(false);
            
            System.Console.WriteLine($"Converting...");
            if (File.Exists(target))
            {
                File.Delete(target);
            }

            // TODO: make pipe configurable through console input
            using var convertedAudio = await pipe
                //.Add(x => x.MonoAudioSampler())
                //.Add(x => x.MonoToStereoAudioSampler())
                //.Add(x => x.SpeedAudioSampler(4))
                //.Add(x => x.SampleRateAudioSampler(44100))
                //.Add(x => x.BitsPerSampleAudioSampler(32)).Add(x => x.VolumeAudioSampler(256)) //HINT: changing height of wave a second time helps
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

        private static string GetDisplayName(string name)
        {
            var method = typeof(AudioSamplerFactory).GetMethod(name);
            return $"{method.Name} ({string.Join(", ", method.GetParameters().Select(p => p.Name + ":" + p.ParameterType.Name))})";
        }

        private static string[] GetAudioSamplerNames()
            => typeof(AudioSamplerFactory).GetMethods().Where(x => !x.IsStatic && x.Name.Contains("Sampler")).Select(x => x.Name).ToArray();
    }
}
