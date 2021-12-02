using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Math;
using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Performance
{
    [SimpleJob(RuntimeMoniker.Net50)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.CoreRt50)]
    [SimpleJob(RuntimeMoniker.CoreRt60)]
    public class AudioPipeBenchmark
    {
        private IAudioFormat targetFormat = new AudioFormat(WellKnownSampleRate.Hz48000, 32, 2, AudioFormatType.Pcm);
        private IAudioFormat? sourceFormat;
        private IAudioContainer? sourceAudio;

        [GlobalSetup]
        public async Task GenerateSinWaveAsync()
        {
            var sampleRate = WellKnownSampleRate.Hz48000;
            var sinWave = WaveBuilder.Sin(sampleRate, 1000, 5);
            sourceFormat = new AudioFormat(sampleRate, 64, 1, AudioFormatType.IeeFloat);
            sourceAudio = await IntermediateAudioContainerBuilder.Build(sourceFormat, sinWave)
                .ToInMemoryContainerAsync()
                .ConfigureAwait(false);
        }

        [Benchmark]
        public async Task ChangeAudioFormatAsync()
        {
            if(sourceFormat == null || sourceAudio == null)
            {
                throw new Exception("Pregenerated data required");
            }

            await sourceAudio
                .ToFormatAsync(targetFormat)
                .ToInMemoryContainerAsync()
                .ConfigureAwait(false);
        }
    }
}
