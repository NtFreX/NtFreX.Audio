﻿using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Samplers
{
    public sealed class AudioSamplerFactory
    {
#pragma warning disable CA1822 // Mark members as static => Instance used by AudioEnvironment
        public AudioSampler MonoAudioSampler() => new MonoAudioSampler();
        public AudioSampler BitsPerSampleAudioSampler(ushort bitsPerSample) => new BitsPerSampleAudioSampler(bitsPerSample);
        public AudioSampler SampleRateAudioSampler(uint sampleRate) => new SampleRateAudioSampler(sampleRate);
        public AudioSampler MonoToStereoAudioSampler() => new MonoToStereoAudioSampler();
        public AudioSampler VolumeAudioSampler(double volumeFactor) => new VolumeAudioSampler(volumeFactor);
        public AudioSampler ShiftWaveAudioSampler(double shiftAdder) => new ShiftWaveAudioSampler(shiftAdder);
#pragma warning restore CA1822 // Mark members as static

        private AudioSamplerFactory() { }

        public static AudioSamplerFactory Instance { [return: NotNull] get; } = new AudioSamplerFactory();
    }
}
