using NtFreX.Audio.Infrastructure.Threading;

namespace NtFreX.Audio.Infrastructure.Container
{
    public interface IIntermediateAudioContainer : IAudioContainer, ISeekableAsyncEnumerable<Sample>
    {
    }
}
