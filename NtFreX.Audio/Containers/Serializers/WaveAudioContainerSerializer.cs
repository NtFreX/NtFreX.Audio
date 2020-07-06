using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
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
    internal class WaveAudioContainerSerializer : AudioContainerSerializer<WaveAudioContainer>
    {
        public override string PreferredFileExtension { [return: NotNull] get; } = "wav";

        [return: NotNull] public override async Task ToStreamAsync([NotNull] WaveAudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var headers = new List<byte>()
                .Concat(container.RiffChunkDescriptor.ChunkId.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(container.RiffChunkDescriptor.ChunkSize.ToByteArray())
                .Concat(container.RiffChunkDescriptor.Format.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(container.FmtSubChunk.Subchunk1Id.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(container.FmtSubChunk.Subchunk1Size.ToByteArray())
                .Concat(container.FmtSubChunk.AudioFormat.ToByteArray())
                .Concat(container.FmtSubChunk.NumChannels.ToByteArray())
                .Concat(container.FmtSubChunk.SampleRate.ToByteArray())
                .Concat(container.FmtSubChunk.ByteRate.ToByteArray())
                .Concat(container.FmtSubChunk.BlockAlign.ToByteArray())
                .Concat(container.FmtSubChunk.BitsPerSample.ToByteArray())
                .Concat(container.UnknownSubChuncks.Select(subChunck =>
                {
                    return new List<byte>(subChunck.SubchunkId.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                        .Concat(subChunck.SubchunkSize.ToByteArray())
                        .Concat(subChunck.SubchunkData);
                }).SelectMany(x => x))
                .Concat(container.DataSubChunk.Subchunk2Id.ToByteArray(isLittleEndian: true /* Doc says it is big endian? */))
                .Concat(container.DataSubChunk.Subchunk2Size.ToByteArray())
                .ToArray();

            await stream.WriteAsync(headers, 0, headers.Length, cancellationToken).ConfigureAwait(false);

            using var readContext = await container.DataSubChunk.Data.AquireAsync(cancellationToken).ConfigureAwait(false);
            await readContext.Data.CopyToAsync(stream, cancellationToken).ConfigureAwait(false); // TODO: stop at data end!
        }

        [return: NotNull] public override async Task<WaveAudioContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            var riff = new RiffChunkDescriptor(
                   chunkId: await stream.ReadStringAsync(length: 4, isLittleEndian: true /* Doc says it is big endian? */, cancellationToken).ConfigureAwait(false),
                   chunkSize: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   format: await stream.ReadStringAsync(length: 4, isLittleEndian: true /* Doc says it is big endian? */, cancellationToken).ConfigureAwait(false));;
            var fmt = new FmtSubChunk(
                   subchunk1Id: await stream.ReadStringAsync(length: 4, isLittleEndian: true /* Doc says it is big endian? */, cancellationToken).ConfigureAwait(false),
                   subchunk1Size: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   audioFormat: await stream.ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   numChannels: await stream.ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   sampleRate: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   /*, byteRate: data.TakeUInt(28),
                   blockAlign: data.TakeUShort(32)*/
                   bitsPerSample: await (await stream.SkipAsync(6, cancellationToken).ConfigureAwait(false)).ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false));

            //TODO: support non seekable streams?
            //TODO: fix all the casting
            var subChunks = new List<UnknownSubChunk>();
            while (stream.Position < stream.Length)
            {
                var chunckId = await stream.ReadStringAsync(length: 4, isLittleEndian: true /* Doc says it is big endian? */, cancellationToken).ConfigureAwait(false);
                if (chunckId == DataSubChunk.DATA)
                {
                    var data = new DataSubChunk(
                          startIndex: stream.Position - 4,
                          subchunk2Id: chunckId,
                          subchunk2Size: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                          data: stream);

                    //TODO: packages after data?
                    return new WaveAudioContainer(riff, fmt, data, subChunks.ToArray());
                } 
                else
                {
                    var size = await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false);
                    var subChunckData = await stream.ReadBytesAsync((int)size, cancellationToken).ConfigureAwait(false);
                    subChunks.Add(new UnknownSubChunk(subchunkId: chunckId, subchunkSize: size, subchunkData: subChunckData));
                }
            }

            throw new ArgumentException("No data sub chunk has been found", nameof(stream));
        }
    }
}
