using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Threading.Extensions;
using System.IO;
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

            var streammEnumerable = stream.ToEnumerable(HeaderSize).SelectManyAsync(x => x, stream.Length - HeaderSize, cancellationToken);
            return IntermediateAudioContainerBuilder.Build(format, streammEnumerable);
        }

        public override async Task ToStreamAsync(IntermediateAudioContainer container, Stream stream, CancellationToken cancellationToken = default)
        {
            var format = container.GetFormat();
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray(format.SampleRate), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray(format.BitsPerSample), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray(format.Channels), cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(EndianAwareBitConverter.ToByteArray((ushort)format.Type), cancellationToken).ConfigureAwait(false);

            await WriteDataAsync(container.GetAsyncAudioEnumerable(cancellationToken), stream, cancellationToken).ConfigureAwait(false);
        }
    }
}
