using NtFreX.Audio.Containers;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal static class AudioFactory
    {
        public static async Task<WaveStreamAudioContainer> GetSampleAudioAsync(string file, CancellationToken cancellationToken)
        {
            System.Console.WriteLine($"Reading...");
            var audio = await AudioEnvironment.Serializer.FromFileAsync(file, cancellationToken).ConfigureAwait(false);
            System.Console.WriteLine($"  Length = {audio.GetLength()}");

            var waveAudio = await AudioEnvironment.Converter.ConvertAsync<WaveStreamAudioContainer>(audio, cancellationToken).ConfigureAwait(false);
            System.Console.WriteLine($"  SampleRate = {waveAudio.Format.SampleRate}");
            System.Console.WriteLine($"  BitsPerSample = {waveAudio.Format.BitsPerSample}");
            System.Console.WriteLine($"  Channels = {waveAudio.Format.Channels}");
            System.Console.WriteLine($"  Type = {waveAudio.Format.Type}");
            return waveAudio;
        }
    }
}
