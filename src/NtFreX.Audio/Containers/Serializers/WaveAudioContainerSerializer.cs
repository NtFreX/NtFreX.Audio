﻿using NtFreX.Audio.Extensions;
using NtFreX.Audio.Helpers;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using NtFreX.Audio.Infrastructure.Threading;
using NtFreX.Audio.Resources;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal class WaveAudioContainerSerializer : AudioContainerSerializer<WaveStreamAudioContainer>
    {
        public override string PreferredFileExtension { [return: NotNull] get; } = "wav";

        [return: NotNull]
        public static async Task<int> WriteHeadersAsync([NotNull] IRiffSubChunk riff, [NotNull] FmtSubChunk fmt, [NotNull] IReadOnlyList<UnknownSubChunk> unknown, [NotNull] IDataSubChunk data, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var headers = GetHeaderBytes(riff, fmt, unknown, data);
            await stream.WriteAsync(headers, cancellationToken).ConfigureAwait(false);
            return headers.Length;
        }

        [return: NotNull]
        public static async Task WriteDataAsync([NotNull] IAsyncEnumerable<Sample> data, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var bufferSize = StreamFactory.GetBufferSize();
            var bufferIndex = 0;
            var buffer = new byte[bufferSize];
            await foreach (var value in data.SelectAsync(x => x.AsByteArray(), cancellationToken).ConfigureAwait(false).WithCancellation(cancellationToken))
            {
                if (bufferSize < bufferIndex + value.Length)
                {
                    await stream.WriteAsync(buffer.AsMemory(0, bufferIndex), cancellationToken).ConfigureAwait(false);
                    bufferIndex = 0;
                }

                value.CopyTo(buffer, bufferIndex);
                bufferIndex += value.Length;
            }

            if (bufferIndex > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, bufferIndex), cancellationToken).ConfigureAwait(false);
            }
        }

        [return: NotNull]
        public static byte[] GetHeaderBytes([NotNull] IRiffSubChunk riff, [NotNull] FmtSubChunk fmt, [NotNull] IReadOnlyList<UnknownSubChunk> unknown, [NotNull] IDataSubChunk data)
        {
            return new List<byte>()
                .Concat(riff.ChunkId.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(riff.ChunkSize.ToByteArray())
                .Concat(riff.Format.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(fmt.ChunkId.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(fmt.ChunkSize.ToByteArray())
                .Concat(((ushort)fmt.Type).ToByteArray())
                .Concat(fmt.Channels.ToByteArray())
                .Concat(fmt.SampleRate.ToByteArray())
                .Concat(fmt.ByteRate.ToByteArray())
                .Concat(fmt.BlockAlign.ToByteArray())
                .Concat(fmt.BitsPerSample.ToByteArray())
                .Concat(unknown.Select(subChunck =>
                {
                    return new List<byte>(subChunck.ChunkId.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                        .Concat(subChunck.ChunkSize.ToByteArray())
                        .Concat(subChunck.SubchunkData);
                }).SelectMany(x => x))
                .Concat(data.ChunkId.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(data.ChunkSize.ToByteArray())
                .ToArray();
        }

        [return: NotNull] 
        public override async Task ToStreamAsync([NotNull] WaveStreamAudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            await WriteHeadersAsync(container.RiffSubChunk, container.FmtSubChunk, container.UnknownSubChunks, container.DataSubChunk, stream, cancellationToken).ConfigureAwait(false);
            await WriteDataAsync(container.GetAudioSamplesAsync(cancellationToken), stream, cancellationToken).ConfigureAwait(false);
        }

        [return: NotNull] public override async Task<WaveStreamAudioContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            StreamBufferSubChunk? data = null;
            FmtSubChunk? fmt = null;
            RiffSubChunk? riff = null;
            var subChunks = new List<UnknownSubChunk>();
            while (stream.Position < stream.Length)
            {
                var chunckId = await stream.ReadStringAsync(length: 4, isLittleEndian: true /* Doc says it is big endian? */, cancellationToken).ConfigureAwait(false);
                if (chunckId == DataSubChunk<ISubChunk>.ChunkIdentifer)
                {
#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
                    data = new StreamBufferSubChunk(
                          startIndex: stream.Position - 4,
                          chunkId: chunckId,
                          chunkSize: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                          data: stream);
#pragma warning restore CA2000 // Dispose objects before losing scope

                    if (stream.CanSeek)
                    {
                        await stream.SkipAsync((int)data.ChunkSize, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        // if the stream is non seekable we break here to ensure we do not read the whole stream
                        // TODO: if any chunks follow after the data chunk they will be ignored
                        break;
                    }
                }
                else if (chunckId == FmtSubChunk.ChunkIdentifier)
                {
                    // TODO: http://www-mmsp.ece.mcgill.ca/Documents/AudioFormats/WAVE/WAVE.html says there are three possible chunks => support all/skip rest of header
                    fmt = new FmtSubChunk(
                       chunkId: chunckId,
                       chunkSize: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                       audioFormat: (AudioFormatType) await stream.ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                       channels: await stream.ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                       sampleRate: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                       /*, byteRate: data.TakeUInt(28),
                       blockAlign: data.TakeUShort(32)*/
                       bitsPerSample: await (await stream.SkipAsync(6, cancellationToken).ConfigureAwait(false)).ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false));
                }
                else if(chunckId == RiffSubChunk.ChunkIdentifierRIFF || chunckId == RiffSubChunk.ChunkIdentifierRIFX)
                {
                    riff = new RiffSubChunk(
                       chunkId: chunckId,
                       chunkSize: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                       format: await stream.ReadStringAsync(length: 4, isLittleEndian: true /* Doc says it is big endian? */, cancellationToken).ConfigureAwait(false));
                }
                else
                {
                    var size = await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false);
                    var subChunckData = await stream.ReadBytesAsync((int)size, cancellationToken).ConfigureAwait(false);
                    subChunks.Add(new UnknownSubChunk(subchunkId: chunckId, subchunkSize: size, subchunkData: subChunckData));
                }
            }

            if(riff == null)
            {
                throw new Exception(ExceptionMessages.WaveAudioContainerNoRiffSubChunk);
            }
            if(fmt == null)
            {
                throw new Exception(ExceptionMessages.WaveAudioContainerNoFmtSubChunk);
            }
            if(data == null)
            {
                throw new Exception(ExceptionMessages.WaveAudioContainerNoDataSubChunk);
            }

#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            var wrapper = new StreamDataSubChunk(data, new SampleDefinition(fmt.Type, fmt.BitsPerSample, riff.IsDataLittleEndian()));
#pragma warning restore CA2000 // Dispose objects before losing scope
            return new WaveStreamAudioContainer(riff, fmt, wrapper, subChunks.ToArray());
        }
    }
}
