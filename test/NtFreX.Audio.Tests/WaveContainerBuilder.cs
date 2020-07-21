﻿using Dasync.Collections;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Tests
{
    internal static class WaveContainerBuilder
    {
        public static WaveEnumerableAudioContainer Build(int sampleCount, ushort bitsPerSample, uint sampleRate, ushort channels = 1, string riffChunkId = RiffChunkDescriptor.ChunkIdentifierRIFF)
        {
            //TODO use WaveEnumerableAudioContainerBuilder delete this
            var byteCount = sampleRate * sampleCount * bitsPerSample / 8;
            var totalSamples = sampleRate * sampleCount;
            return new WaveEnumerableAudioContainer(
                   new RiffChunkDescriptor(riffChunkId, 0, RiffChunkDescriptor.WAVE),
                   new FmtSubChunk("fmt ", 16, AudioFormatType.PCM, channels, sampleRate, bitsPerSample),
                   new EnumerableDataSubChunk(
                       "data",
                       (uint)byteCount,
                       Enumerable.Repeat(0L, (int)totalSamples).Select(x => x.ToByteArray(bitsPerSample / 8)).ToAsyncEnumerable()),
                   new List<UnknownSubChunk>());
        }
    }
}
