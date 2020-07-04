using System;
using System.Threading.Tasks;

namespace NtFreX.Audio.Containers
{
    public abstract class AudioContainer
    {
        public abstract TimeSpan GetLength();

        public Task ToFileAsync(string path) => AudioEnvironment.Serializer.ToFileAsync(path, this);
    }
}
