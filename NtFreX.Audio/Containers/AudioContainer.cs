using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public abstract class AudioContainer
    {
        [return:NotNull] public abstract TimeSpan GetLength();

        [return: NotNull] public Task ToFileAsync([NotNull] string path, [MaybeNull] CancellationToken cancellationToken = default) => AudioEnvironment.Serializer.ToFileAsync(path, this, cancellationToken);
    }
}
