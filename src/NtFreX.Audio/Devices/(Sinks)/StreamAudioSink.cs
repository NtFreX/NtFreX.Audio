using NtFreX.Audio.AdapterInfrastructure;
using NtFreX.Audio.Containers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NtFreX.Audio.Devices
{
    public class StreamAudioSink : IAudioSink
    {
        private uint size = 0;

        public Stream Stream { get; }

        public StreamAudioSink(Stream stream)
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
            size += (uint) (data == null ? 0 : data.Length);
            Stream.Write(data);
        }

        /// <summary>
        /// Writes the total data size back into the header
        /// </summary>
        public void Finish()
        {
            var sizeBuffer = BitConverter.GetBytes(size);

            // TODO: find data pos some way other
            Stream.Seek(40, SeekOrigin.Begin);
            Stream.Write(sizeBuffer, 0, sizeBuffer.Length);
        }
    }
}
