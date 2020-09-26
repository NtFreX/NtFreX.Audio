using NtFreX.Audio.Extensions;
using NtFreX.Audio.Helpers;
using NtFreX.Audio.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal class IntermediateAudioContainerSerializer : AudioContainerSerializer<IntermediateAudioContainer>
    {
        public override string PreferredFileExtension => ".audx";

        private const int HeaderSize = 10;

        public override async Task<IntermediateAudioContainer> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            uint sampleRate = await stream.ReadUInt32Async(cancellationToken: cancellationToken).ConfigureAwait(false);
            ushort bitsPerSample = await stream.ReadUInt16Async(cancellationToken: cancellationToken).ConfigureAwait(false);
            ushort channels = await stream.ReadUInt16Async(cancellationToken: cancellationToken).ConfigureAwait(false);
            ushort type = await stream.ReadUInt16Async(cancellationToken: cancellationToken).ConfigureAwait(false);
            var format = new AudioFormat(sampleRate, bitsPerSample, channels, (AudioFormatType) type);

            return IntermediateAudioContainerBuilder.Build(format, ReadStreamAsync(stream, cancellationToken), stream.Length - HeaderSize);
        }

        public override async Task ToStreamAsync(IntermediateAudioContainer container, Stream stream, CancellationToken cancellationToken = default)
        {
            var format = container.GetFormat();
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray(format.SampleRate), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray(format.BitsPerSample), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray(format.Channels), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray((ushort)format.Type), cancellationToken).ConfigureAwait(false);

            await WriteStreamAsync(stream, container.GetAsyncAudioEnumerator(cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private static async Task WriteStreamAsync(Stream stream, IAsyncEnumerator<IReadOnlyList<byte>?> data, CancellationToken cancellationToken)
        {
            while (await data.MoveNextAsync().ConfigureAwait(false))
            {
                await stream.WriteAsync(data.Current.ToArray(), cancellationToken).ConfigureAwait(false);
            }
        }

        private static async IAsyncEnumerable<byte> ReadStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            var bufferSize = StreamFactory.GetBufferSize();
            var buffer = new byte[bufferSize];
            int size;
            do
            {
                size = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                for (var i = 0; i < size; i++)
                {
                    yield return buffer[i];
                }
            }
            while (size == bufferSize);
        }
    }
}
