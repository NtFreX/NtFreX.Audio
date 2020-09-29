using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Samplers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
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
                var samplers = GetAudioSamplers();
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
                        var parameters = samplers[number - 1].GetParameters();
                        var args = parts.Skip(1).Select((x, i) => Convert.ChangeType(x.Contains('.', StringComparison.Ordinal) ? decimal.Parse(x, CultureInfo.InvariantCulture) : long.Parse(x, CultureInfo.InvariantCulture), parameters[i].ParameterType, CultureInfo.InvariantCulture)).ToArray();
                        pipe.Add(x => (samplers[number - 1].Invoke(x, args) ?? throw new Exception()) as AudioSampler ?? throw new Exception());
                    }
                    catch (Exception exce)
                    {
                        System.Console.WriteLine(exce.Message);
                    }
                }
            }

            System.Console.WriteLine($"Converting...");
            if (File.Exists(target))
            {
                File.Delete(target);
            }

            await using var audio = await AudioFactory
                .GetSampleAudioAsync(file, cancellationToken)
                .ConvertAsync<IntermediateEnumerableAudioContainer>(cancellationToken)
                .RunAudioPipeAsync(pipe, cancellationToken)
                .LogProgressAsync(ConsoleProgressBar.LogProgress, cancellationToken)
                .ToFileAsync(target, FileMode.OpenOrCreate, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            
            System.Console.WriteLine();
        }

        private static string GetDisplayName(MethodInfo sampler)
            => $"{sampler?.Name} ({string.Join(", ", sampler?.GetParameters().Select(p => p.Name + ":" + p.ParameterType.Name) ?? Array.Empty<string>())})";

        private static MethodInfo[] GetAudioSamplers()
            => typeof(AudioSamplerFactory).GetMethods().Where(x => !x.IsStatic && !string.IsNullOrEmpty(x.Name) && x.Name.Contains("Sampler", StringComparison.Ordinal)).ToArray();
    }
}
