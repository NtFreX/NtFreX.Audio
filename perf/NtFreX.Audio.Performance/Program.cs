﻿using BenchmarkDotNet.Running;

namespace NtFreX.Audio.Performance
{
    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<AudioPipeBenchmark>();
            BenchmarkRunner.Run<SampleRateAudioSamplerBenchmark>();
        }
    }
}
