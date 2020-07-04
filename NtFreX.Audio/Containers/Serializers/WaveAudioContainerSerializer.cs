using NtFreX.Audio.Extensions;
using NtFreX.Audio.Math;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal class WaveAudioContainerSerializer : AudioContainerSerializer<WaveAudioContainer>
    {
        public override string PreferredFileExtension => "wav";

        public override async Task ToStreamAsync(WaveAudioContainer container, Stream stream, CancellationToken cancellationToken = default)
        {
            var headers = new List<byte>()
                .Concat(container.RiffChunkDescriptor.ChunkId.ToByteArray(isLittleEndian: false))
                .Concat(container.RiffChunkDescriptor.ChunkSize.ToByteArray())
                .Concat(container.RiffChunkDescriptor.Format.ToByteArray(isLittleEndian: false))
                .Concat(container.FmtSubChunk.Subchunk1Id.ToByteArray(isLittleEndian: false))
                .Concat(container.FmtSubChunk.Subchunk1Size.ToByteArray())
                .Concat(container.FmtSubChunk.AudioFormat.ToByteArray())
                .Concat(container.FmtSubChunk.NumChannels.ToByteArray())
                .Concat(container.FmtSubChunk.SampleRate.ToByteArray())
                .Concat(container.FmtSubChunk.ByteRate.ToByteArray())
                .Concat(container.FmtSubChunk.BlockAlign.ToByteArray())
                .Concat(container.FmtSubChunk.BitsPerSample.ToByteArray())
                .Concat(container.RiffSubChuncks.Select(subChunck =>
                {
                    return new List<byte>(subChunck.SubchunkId.ToByteArray(isLittleEndian: false))
                        .Concat(subChunck.SubchunkSize.ToByteArray())
                        .Concat(subChunck.SubchunkData);
                }).SelectMany(x => x))
                .Concat(container.DataSubChunk.Subchunk2Id.ToByteArray(isLittleEndian: false))
                .Concat(container.DataSubChunk.Subchunk2Size.ToByteArray())
                .ToArray();

            await stream.WriteAsync(headers, 0, headers.Length, cancellationToken).ConfigureAwait(false);

            using var readContext = await container.DataSubChunk.Data.AquireAsync(cancellationToken).ConfigureAwait(false);
            await readContext.Data.CopyToAsync(stream, cancellationToken).ConfigureAwait(false); // TODO: stop at data end!
        }

        public override async Task<WaveAudioContainer> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var riff = new RiffChunkDescriptor(
                   chunkId: await stream.ReadInt32Async(isLittleEndian: false, cancellationToken).ConfigureAwait(false),
                   chunkSize: await stream.ReadInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   format: await stream.ReadInt32Async(isLittleEndian: false, cancellationToken).ConfigureAwait(false));
            var fmt = new FmtSubChunk(
                   subchunk1Id: await stream.ReadInt32Async(isLittleEndian: false, cancellationToken).ConfigureAwait(false),
                   subchunk1Size: await stream.ReadInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   audioFormat: await stream.ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   numChannels: await stream.ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   sampleRate: await stream.ReadInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                   /*, byteRate: data.TakeUInt(28),
                   blockAlign: data.TakeUShort(32)*/
                   bitsPerSample: await (await stream.SkipAsync(6, cancellationToken).ConfigureAwait(false)).ReadUInt16Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false));

            //TODO: support non seekable streams?
            //TODO: fix all the casting
            DataSubChunk data = null;
            var subChunks = new List<RiffSubChunk>();
            while (stream.Position < stream.Length)
            {
                var chunckId = await stream.ReadInt32Async(isLittleEndian: false, cancellationToken).ConfigureAwait(false);
                if (Encoding.ASCII.GetString(BitConverter.GetBytes(chunckId).Reverse().ToArray()) == DataSubChunk.DATA)
                {
                    data = new DataSubChunk(
                       startIndex: (int) stream.Position - 4,
                       subchunk2Id: chunckId,
                       subchunk2Size: await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false),
                       data: stream);

                    //TODO: packages after data?
                    break;
                } 
                else
                {
                    var size = await stream.ReadUInt32Async(isLittleEndian: true, cancellationToken).ConfigureAwait(false);
                    var subChunckData = await stream.ReadBytesAsync((int)size, cancellationToken).ConfigureAwait(false);
                    subChunks.Add(new RiffSubChunk(subchunkId: chunckId, subchunkSize: size, subchunkData: subChunckData));
                }
            }

            return new WaveAudioContainer(riff, fmt, data, subChunks.ToArray());
        }
    }
}
