﻿using NtFreX.Audio.Infrastructure;

namespace NtFreX.Audio.Samplers
{
    public sealed class AudioSamplerFactory
    {
#pragma warning disable CA1822 // Mark members as static => Instance used by AudioEnvironment
        public AudioSampler ChannelAudioSampler(Speakers speaker) => new ChannelAudioSampler(speaker);
        public AudioSampler ChannelAudioSampler(ushort channels) => new ChannelAudioSampler(channels);
        public AudioSampler ToMonoAudioSampler() => new ToMonoAudioSampler();
        public AudioSampler FromMonoAudioSampler(int taretChannels) => new FromMonoAudioSampler(taretChannels);
        public AudioSampler BitsPerSampleAudioSampler(ushort bitsPerSample) => new BitsPerSampleAudioSampler(bitsPerSample);
        public AudioSampler SampleRateAudioSampler(uint sampleRate) => new SampleRateAudioSampler(sampleRate);
        public AudioSampler PcmToFloatAudioSampler() => new PcmToFloatAudioSampler();
        public AudioSampler FloatToPcmAudioSampler() => new FloatToPcmAudioSampler();
        public AudioSampler VolumeAudioSampler(double volumeFactor) => new VolumeAudioSampler(volumeFactor);
        public AudioSampler SpeedAudioSampler(double speedFactor) => new SpeedAudioSampler(speedFactor);
        public AudioSampler ShiftWaveAudioSampler(double shiftAdder) => new ShiftWaveAudioSampler(shiftAdder);
#pragma warning restore CA1822 // Mark members as static

        private AudioSamplerFactory() { }

        public static AudioSamplerFactory Instance { get; } = new AudioSamplerFactory();
    }
}
