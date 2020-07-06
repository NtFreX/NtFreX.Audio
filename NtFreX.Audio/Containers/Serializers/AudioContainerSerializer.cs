using NtFreX.Audio.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal abstract class AudioContainerSerializer<TContainer> : IAudioContainerSerializer
        where TContainer : AudioContainer
    {
        public abstract string PreferredFileExtension { [return:NotNull] get; }

        [return:NotNull] public async Task ToFileAsync([NotNull] string path, [NotNull] TContainer container, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using var steam = File.OpenWrite(path);
            await ToStreamAsync(container, steam, cancellationToken).ConfigureAwait(false);
        }
        [return:NotNull] public Task<TContainer> FromFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default) 
            => FromStreamAsync(File.OpenRead(path), cancellationToken);

        [return:NotNull] public async Task<byte[]> ToDataAsync([NotNull] TContainer container, [MaybeNull] CancellationToken cancellationToken = default) 
        {
            using var stream = StreamFactory.Instance.ResolveNewStreamForInMemory();
            await ToStreamAsync(container, stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            return buffer;
        }
        [return:NotNull] public async Task<TContainer> FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken = default)
        {
            using var stream = await StreamFactory.Instance.WriteToNewStreamAsync(data).ConfigureAwait(false);
            return await FromStreamAsync(stream, cancellationToken).ConfigureAwait(false);
        }

        [return:NotNull] public abstract Task ToStreamAsync([NotNull] TContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);
        [return:NotNull] public abstract Task<TContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);

        [return:NotNull] async Task<AudioContainer> IAudioContainerSerializer.FromFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken) => await FromFileAsync(path, cancellationToken).ConfigureAwait(false);
        [return:NotNull] async Task<AudioContainer> IAudioContainerSerializer.FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken) => await FromDataAsync(data, cancellationToken).ConfigureAwait(false);
        [return:NotNull] async Task<AudioContainer> IAudioContainerSerializer.FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken) => await FromStreamAsync(stream, cancellationToken).ConfigureAwait(false);

        [return:NotNull] public Task ToFileAsync([NotNull] string path, [NotNull] AudioContainer container, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToFileAsync(path, genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        [return:NotNull] public Task<byte[]> ToDataAsync([NotNull] AudioContainer container, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToDataAsync(genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        [return:NotNull] public Task ToStreamAsync([NotNull] AudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToStreamAsync(genericContainer, stream, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
    }
}
