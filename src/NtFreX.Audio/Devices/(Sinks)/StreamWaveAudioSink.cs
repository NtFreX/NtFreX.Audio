using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Containers.Serializers;
using NtFreX.Audio.Containers.Wave;
using NtFreX.Audio.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public class StreamWaveAudioSink : IAudioSink, IAsyncDisposable
    {
        private uint size;
        private bool isDisposed;

        public Stream Stream { get; }

        protected StreamWaveAudioSink(Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!stream.CanSeek)
            {
                throw new ArgumentException("Only seekable streams are supported", nameof(stream));
            }

            Stream = stream;
        }

        public static async Task<StreamWaveAudioSink> CreateAsync(Stream stream, IAudioFormat format)
        {
            var sink = new StreamWaveAudioSink(stream);
            await sink.InitializeAsync(format).ConfigureAwait(false);
            return sink;
        }

        public void DataReceived(byte[] data)
        {
            if(isDisposed)
            {
                throw new Exception("The sink has allready been closed");
            }

            size += (uint) (data == null ? 0 : data.Length);
            Stream.Write(data);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore(true).ConfigureAwait(false);

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        protected async Task InitializeAsync(IAudioFormat format)
        {
            _ = format ?? throw new ArgumentNullException(nameof(format));

            await using var container = WaveAudioContainerBuilder.Build(Array.Empty<byte>(), format);

            await WaveAudioContainerSerializer.WriteHeadersAsync(
                container.RiffSubChunk,
                container.FmtSubChunk,
                container.UnknownSubChunks,
                container.DataSubChunk,
                Stream).ConfigureAwait(false);
        }

        /// <summary>
        /// Writes the total data size back into the wave riff header
        /// </summary>
        /// <param name="disposing">is comming from dipose call</param>
        /// <returns>Task</returns>
        protected virtual async ValueTask DisposeAsyncCore(bool disposing)
        {
            if (disposing && !isDisposed)
            {
                isDisposed = true;

                var dataSizeBuffer = BitConverter.GetBytes(size);
                var totalSizeBuffer = BitConverter.GetBytes((uint)(size + WaveAudioContainer.DefaultHeaderSize));

                await Stream.FlushAsync().ConfigureAwait(false);

                // TODO: find data pos some way other
                // riff file size should allways be at 4
                Stream.Seek(4, SeekOrigin.Begin);
                Stream.Write(totalSizeBuffer, 0, totalSizeBuffer.Length);

                // data size could be in another place as 40 when unknown sub chunks exist
                Stream.Seek(40, SeekOrigin.Begin);
                Stream.Write(dataSizeBuffer, 0, dataSizeBuffer.Length);

                await Stream.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}
