using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Infrastructure
{
    public interface IAudioContainer
    {
        [return: NotNull] TimeSpan GetLength();
    }
}
