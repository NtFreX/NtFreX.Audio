using NtFreX.Audio.Containers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{
    internal static class AudioFactory
    {
        public static async Task<WaveStreamAudioContainer> GetSampleAudioAsync(string file, CancellationToken cancellationToken)
        {
            System.Console.WriteLine($"Reading...");
            var audio = await AudioEnvironment.Serializer.FromFileAsync(file, cancellationToken).ConfigureAwait(false);
            System.Console.WriteLine($"  Length = {audio.GetLength()}");

            // TODO: implement/call converter library to get wave file
            if (audio is WaveStreamAudioContainer waveAudioContainer)
            {
                return waveAudioContainer;
            }

            throw new Exception("The sample audio container is not supported");
        }
    }
}
