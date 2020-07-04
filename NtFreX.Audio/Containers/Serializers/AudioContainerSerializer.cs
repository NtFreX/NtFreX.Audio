using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal abstract class AudioContainerSerializer<TContainer> : IAudioContainerSerializer
        where TContainer : AudioContainer
    {
        public abstract string PreferredFileExtension { get; }

        public async Task ToFileAsync(string path, TContainer container, CancellationToken cancellationToken = default)
        {
            using var steam = File.OpenWrite(path);
            await ToStreamAsync(container, steam, cancellationToken).ConfigureAwait(false);
        }
        public Task<TContainer> FromFileAsync(string path, CancellationToken cancellationToken = default) => FromStreamAsync(File.OpenRead(path), cancellationToken);

        public async Task<byte[]> ToDataAsync(TContainer container, CancellationToken cancellationToken = default) 
        {
            using var stream = new MemoryStream();
            await ToStreamAsync(container, stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            return buffer;
        }
        public Task<TContainer> FromDataAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            using var stream = new MemoryStream(data);
            return FromStreamAsync(stream, cancellationToken);
        }

        public abstract Task ToStreamAsync(TContainer container, Stream stream, CancellationToken cancellationToken = default);
        public abstract Task<TContainer> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default);

        async Task<AudioContainer> IAudioContainerSerializer.FromFileAsync(string path, CancellationToken cancellationToken = default) => await FromFileAsync(path, cancellationToken).ConfigureAwait(false);
        async Task<AudioContainer> IAudioContainerSerializer.FromDataAsync(byte[] data, CancellationToken cancellationToken = default) => await FromDataAsync(data, cancellationToken).ConfigureAwait(false);
        async Task<AudioContainer> IAudioContainerSerializer.FromStreamAsync(Stream stream, CancellationToken cancellationToken) => await FromStreamAsync(stream, cancellationToken).ConfigureAwait(false);

        public Task ToFileAsync(string path, AudioContainer container, CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToFileAsync(path, genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        public Task<byte[]> ToDataAsync(AudioContainer container, CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToDataAsync(genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        public Task ToStreamAsync(AudioContainer container, Stream stream, CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToStreamAsync(genericContainer, stream, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
    }
}
