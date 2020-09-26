using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Infrastructure.Container;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Console
{
    internal static class AudioFactory
    {
        public static async Task<IAudioContainer> GetSampleAudioAsync(string file, CancellationToken cancellationToken)
        {
            System.Console.WriteLine($"Reading...");
            // TODO: choose serializer
            var audio = await AudioEnvironment.Serializer.FromFileAsync(file, Path.GetExtension(file), cancellationToken).ConfigureAwait(false);
            System.Console.WriteLine($"  Length = {audio.GetLength()}");

            PrintAudioFormat(audio.GetFormat());
            return audio;
        }

        public static void PrintAudioFormat(IAudioFormat format)
        {
            System.Console.WriteLine($"  SampleRate = {format.SampleRate}");
            System.Console.WriteLine($"  BitsPerSample = {format.BitsPerSample}");
            System.Console.WriteLine($"  Channels = {format.Channels}");
            System.Console.WriteLine($"  Type = {format.Type}");
        }
    }
}
