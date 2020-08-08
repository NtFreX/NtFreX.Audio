using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Containers;
using NtFreX.Audio.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public class StreamWaveAudioSink : IAudioSink
    {
        private uint size = 0;
        private bool isFinished = false;

        public Stream Stream { get; }

        public StreamWaveAudioSink(Stream stream)
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));

            if (!stream.CanSeek)
            {
                throw new ArgumentException("Only seekable streams are supported", nameof(stream));
            }

            Stream = stream;
        }

        public async Task InitializeAsync(AudioFormat format)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope => do not dispose stream!
            await WaveEnumerableAudioContainerBuilder
                .Build(format, Array.Empty<byte>())
                .ToStreamAsync(Stream)
                .ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope
        }

        public void DataReceived(byte[] data)
        {
            if(isFinished)
            {
                throw new Exception("The sink has allready been closed");
            }

            size += (uint) (data == null ? 0 : data.Length);
            Stream.Write(data);
        }

        /// <summary>
        /// Writes the total data size back into the wave riff header
        /// </summary>
        public void Finish()
        {
            isFinished = true;

            var dataSizeBuffer = BitConverter.GetBytes(size);
            var totalSizeBuffer = BitConverter.GetBytes((uint) (size + WaveAudioContainer<IDataSubChunk>.DefaultHeaderSize));

            // TODO: find data pos some way other
            // riff file size should allways be at 4
            Stream.Seek(4, SeekOrigin.Begin);
            Stream.Write(totalSizeBuffer, 0, totalSizeBuffer.Length);

            // data size could be in another place as 40 when unknown sub chunks exist
            Stream.Seek(40, SeekOrigin.Begin);
            Stream.Write(dataSizeBuffer, 0, dataSizeBuffer.Length);
        }
    }
}
