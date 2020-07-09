using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Containers
{
    public interface IAudioContainer
    {
        [return: NotNull] TimeSpan GetLength();
    }
}
