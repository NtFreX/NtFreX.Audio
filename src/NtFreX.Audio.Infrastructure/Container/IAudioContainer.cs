using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Infrastructure.Container
{
    public interface IAudioContainer
    {
        [return: NotNull] TimeSpan GetLength();
    }
}
