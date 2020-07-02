namespace NtFreX.Audio.Containers.Serializers
{
    internal interface IAudioContainerSerializer
    {
        string PreferredFileExtension { get; }

        void ToFile(string path, AudioContainer container);
        AudioContainer FromFile(string path);

        byte[] ToData(AudioContainer container);
        AudioContainer FromData(byte[] data);
    }
}
