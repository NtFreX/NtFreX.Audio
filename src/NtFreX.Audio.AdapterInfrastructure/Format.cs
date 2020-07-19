namespace NtFreX.Audio.AdapterInfrastructure
{
    //TODO: change interfaces and work with one format? (make it possible to interact without wave audioContainer)
    public sealed class Format
    {
        public uint SampleRate { get; }
        public ushort BitsPerSample { get; }

        public Format(uint sampleRate, ushort bitPerSample)
        {
            SampleRate = sampleRate;
            BitsPerSample = bitPerSample;
        }
    }
}
