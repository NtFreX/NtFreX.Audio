using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal abstract class AudioContainerSerializer<TContainer> : IAudioContainerSerializer
        where TContainer : IAudioContainer
    {
        public abstract string PreferredFileExtension { [return:NotNull] get; }

        public async Task ToFileAsync(string path, TContainer container, CancellationToken cancellationToken = default)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using var steam = File.OpenWrite(path);
            await ToStreamAsync(container, steam, cancellationToken).ConfigureAwait(false);
        }
        public Task<TContainer> FromFileAsync(string path, CancellationToken cancellationToken = default)
#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            => FromStreamAsync(File.OpenRead(path), cancellationToken);
#pragma warning restore CA2000 // Dispose objects before losing scope

        public async Task<byte[]> ToDataAsync(TContainer container, CancellationToken cancellationToken = default) 
        {
            using var stream = new MemoryStream();
            await ToStreamAsync(container, stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            return stream.ToArray();
        }
        public async Task<TContainer> FromDataAsync(byte[] data, CancellationToken cancellationToken = default)
#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            => await FromStreamAsync(new MemoryStream(data), cancellationToken).ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope

        public abstract Task ToStreamAsync(TContainer container, Stream stream, CancellationToken cancellationToken = default);
        public abstract Task<TContainer> FromStreamAsync(Stream stream, CancellationToken cancellationToken = default);

        async Task<IAudioContainer> IAudioContainerSerializer.FromFileAsync(string path, CancellationToken cancellationToken) 
            => await FromFileAsync(path, cancellationToken).ConfigureAwait(false);
        async Task<IAudioContainer> IAudioContainerSerializer.FromDataAsync(byte[] data, CancellationToken cancellationToken) 
            => await FromDataAsync(data, cancellationToken).ConfigureAwait(false);
        async Task<IAudioContainer> IAudioContainerSerializer.FromStreamAsync(Stream stream, CancellationToken cancellationToken) 
            => await FromStreamAsync(stream, cancellationToken).ConfigureAwait(false);

        public Task ToFileAsync(string path, IAudioContainer container, CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToFileAsync(path, genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        public Task<byte[]> ToDataAsync(IAudioContainer container, CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToDataAsync(genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        public Task ToStreamAsync(IAudioContainer container, Stream stream, CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToStreamAsync(genericContainer, stream, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
    }
}
