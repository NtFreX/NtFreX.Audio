using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Infrastructure
{
    public static class SampleExtensions
    {
        public static Sample Average(this IEnumerable<Sample> samples)
        {
            var data = samples.ToArray();
            return data.Aggregate((a, b) => a + b) / data.Length;
        }
    }
}
