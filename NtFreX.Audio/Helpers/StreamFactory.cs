namespace NtFreX.Audio.Helpers
{
    //TODO: use factory everywhere for buffersize and stream resolver
    internal sealed class StreamFactory
    {
        public static int GetBufferSize() => 2000;
    }
}
