using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Containers.Wave
{
    public static class WaveAudioContainerBuilder
    {
        public static WaveAudioContainer Build(byte[] data, IAudioFormat format, bool isDataLittleEndian = true)
        {
            _ = data ?? throw new ArgumentNullException(nameof(data));
            _ = format ?? throw new ArgumentNullException(nameof(format));

            // TODO: make seekable
            var enumerable = data
                .ToAsyncEnumerable()
                .GroupByLengthAsync(format.BytesPerSample)
                .SelectAsync(x => (IReadOnlyList<byte>?)x.ToList())
                .ToNonSeekable(data.Length / format.BytesPerSample);

            return Build(
                enumerable,
                (uint) data.Length,
                format,
                isDataLittleEndian);
        }

        public static WaveAudioContainer Build(ISeekableAsyncEnumerable<IReadOnlyList<byte>> data, uint size, IAudioFormat format, bool isDataLittleEndian = true)
        {
            _ = data ?? throw new ArgumentNullException(nameof(data));
            _ = format ?? throw new ArgumentNullException(nameof(format));

            var riffChunk = new RiffSubChunk(isDataLittleEndian ? RiffSubChunk.ChunkIdentifierRIFF : RiffSubChunk.ChunkIdentifierRIFX, /* size of file minus 8: 36 + data in default case */ (uint)WaveAudioContainer.DefaultHeaderSize, RiffSubChunk.WAVE);
            var fmtChunk = new FmtSubChunk(FmtSubChunk.ChunkIdentifier, FmtSubChunk.FmtChunkSize, format.Type, format.Channels, format.SampleRate, format.BitsPerSample);
            var dataChunk = new DataSubChunk(DataSubChunk.ChunkIdentifer, data, size);

            return new WaveAudioContainer(
                riffChunk,
                fmtChunk,
                dataChunk,
                new List<UnknownSubChunk>());
        }
    }
}
