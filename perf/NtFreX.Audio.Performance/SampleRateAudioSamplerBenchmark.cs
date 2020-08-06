using BenchmarkDotNet.Attributes;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using NtFreX.Audio.Samplers;
using System.Threading.Tasks;

namespace NtFreX.Audio.Performance
{
    public class SampleRateAudioSamplerBenchmark
    {
        [Benchmark]
        public async Task SampleFrom44100To48000_32Bit_2Channels_Pcm_5Sec()
        {
            var isLittleEndian = true;
            uint sampleRate = 44100;
            ushort bitsPerSample = 32;
            ushort channels = 2;
            var length = 5;
            var format = new AudioFormat(sampleRate, bitsPerSample, channels, AudioFormatType.Pcm);
            // TODO: better test data
            var audio = WaveEnumerableAudioContainerBuilder.Build(format, length, isLittleEndian);
            var sampler = new SampleRateAudioSampler(48000);

            await sampler.SampleAsync(audio).ConfigureAwait(false);
        }

        [Benchmark]
        public async Task SampleFrom48000To44100_32Bit_2Channels_Pcm_5Sec()
        {
            var isLittleEndian = true;
            uint sampleRate = 48000;
            ushort bitsPerSample = 32;
            ushort channels = 2;
            var length = 5;
            var format = new AudioFormat(sampleRate, bitsPerSample, channels, AudioFormatType.Pcm);
            // TODO: better test data
            var audio = WaveEnumerableAudioContainerBuilder.Build(format, length, isLittleEndian);
            var sampler = new SampleRateAudioSampler(44100);

            await sampler.SampleAsync(audio).ConfigureAwait(false);
        }
    }
}
