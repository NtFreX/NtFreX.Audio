using System.Linq;

namespace NtFreX.Audio.Samplers
{
    public class MonoAudioSampler
    {
        public static int[] InterleaveChannelData(int[][] channels)
        {
            var interleavedChannelData = new int[channels.First().Length];
            for (var i = 0; i < interleavedChannelData.Length; i += 1)
            {
                interleavedChannelData[i] = (int)channels.Select(x => x[i]).Average();
            }

            return interleavedChannelData;
        }
    }
}
