using System;

namespace NtFreX.Audio.Containers
{
    public abstract class AudioContainer
    {
        public abstract TimeSpan GetLength();

        public void ToFile(string path) => AudioEnvironment.Serializer.ToFile(path, this);
    }
}
