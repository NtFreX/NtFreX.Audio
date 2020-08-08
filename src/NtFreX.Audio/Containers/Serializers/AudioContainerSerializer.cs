using NtFreX.Audio.Infrastructure.Container;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers.Serializers
{
    internal abstract class AudioContainerSerializer<TContainer> : IAudioContainerSerializer
        where TContainer : IStreamAudioContainer
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
#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            => FromStreamAsync(File.OpenRead(path), cancellationToken);
#pragma warning restore CA2000 // Dispose objects before losing scope

        [return:NotNull] public async Task<byte[]> ToDataAsync([NotNull] TContainer container, [MaybeNull] CancellationToken cancellationToken = default) 
        {
            using var stream = new MemoryStream();
            await ToStreamAsync(container, stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            return stream.ToArray();
        }
        [return:NotNull] public async Task<TContainer> FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken = default)
#pragma warning disable CA2000 // Dispose objects before losing scope => The method that raised the warning returns an IDisposable object that wraps your object
            => await FromStreamAsync(new MemoryStream(data), cancellationToken).ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope

        [return:NotNull] public abstract Task ToStreamAsync([NotNull] TContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);
        [return:NotNull] public abstract Task<TContainer> FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default);

        [return:NotNull] async Task<IStreamAudioContainer> IAudioContainerSerializer.FromFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken) => await FromFileAsync(path, cancellationToken).ConfigureAwait(false);
        [return:NotNull] async Task<IStreamAudioContainer> IAudioContainerSerializer.FromDataAsync([NotNull] byte[] data, [MaybeNull] CancellationToken cancellationToken) => await FromDataAsync(data, cancellationToken).ConfigureAwait(false);
        [return:NotNull] async Task<IStreamAudioContainer> IAudioContainerSerializer.FromStreamAsync([NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken) => await FromStreamAsync(stream, cancellationToken).ConfigureAwait(false);

        [return:NotNull] public Task ToFileAsync([NotNull] string path, [NotNull] IStreamAudioContainer container, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToFileAsync(path, genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        [return:NotNull] public Task<byte[]> ToDataAsync([NotNull] IStreamAudioContainer container, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToDataAsync(genericContainer, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        [return:NotNull] public Task ToStreamAsync([NotNull] IStreamAudioContainer container, [NotNull] Stream stream, [MaybeNull] CancellationToken cancellationToken = default)
        {
            if (container is TContainer genericContainer)
            {
                return ToStreamAsync(genericContainer, stream, cancellationToken);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
    }
}
