using System;
using System.IO;

namespace NtFreX.Audio.Containers.Serializers
{
    internal abstract class AudioContainerSerializer<TContainer> : IAudioContainerSerializer
        where TContainer : AudioContainer
    {
        public abstract string PreferredFileExtension { get; }

        public void ToFile(string path, TContainer container) => File.WriteAllBytes(path, ToData(container));
        public TContainer FromFile(string path) => FromData(File.ReadAllBytes(path));

        public abstract byte[] ToData(TContainer container);
        public abstract TContainer FromData(byte[] data);

        AudioContainer IAudioContainerSerializer.FromFile(string path) => FromFile(path);
        AudioContainer IAudioContainerSerializer.FromData(byte[] data) => FromData(data);

        public void ToFile(string path, AudioContainer container)
        {
            if (container is TContainer genericContainer)
            {
                ToFile(path, genericContainer);
                return;
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }
        public byte[] ToData(AudioContainer container)
        {
            if (container is TContainer genericContainer)
            {
                return ToData(genericContainer);
            }

            throw new ArgumentException($"The container has to be of type {typeof(TContainer)}.", nameof(container));
        }

    }
}
