using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Samplers
{
    public sealed class AudioSamplerFactory
    {
        [return: NotNull] public AudioSampler MonoAudioSampler() => new MonoAudioSampler();
        [return: NotNull] public AudioSampler BitsPerSampleAudioSampler(ushort bitsPerSample) => new BitsPerSampleAudioSampler(bitsPerSample);
        [return: NotNull] public AudioSampler SampleRateAudioSampler(uint sampleRate) => new SampleRateAudioSampler(sampleRate);

        private AudioSamplerFactory() { }

        public static AudioSamplerFactory Instance { [return: NotNull] get; } = new AudioSamplerFactory();
    }
}
