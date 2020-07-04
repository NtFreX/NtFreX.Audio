using NtFreX.Audio.Math;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NtFreX.Audio.Samplers
{
    public class MonoAudioSampler
    {
        public static async Task<int[]> InterleaveChannelDataAsync(IAsyncEnumerable<byte[]> data, int channels)
        {
            var interleavedChannelData = new List<int>();
            var temp = new int[channels];
            var counter = 0;
            await foreach(var value in data.ConfigureAwait(false))
            {
                temp[counter++] = value.ToInt32();
                if(counter == channels)
                {
                    interleavedChannelData.Add((int)temp.Average());
                    counter = 0;
                }
            }
            return interleavedChannelData.ToArray();
        }
    }
}
