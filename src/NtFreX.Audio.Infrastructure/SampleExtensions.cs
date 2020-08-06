using System;
using System.Collections.Generic;
using System.Linq;

namespace NtFreX.Audio.Infrastructure
{
    public static class SampleExtensions
    {
        public static Sample Average(this IEnumerable<Sample> samples)
        {
            decimal sum = 0;
            var data = samples.ToArray();
            foreach (var sample in data)
            {
                if(data[0].Definition != sample.Definition)
                {
                    throw new Exception("The given samples are not of the same type");
                }
                sum += (decimal) sample.Value;
            }
            return new Sample((long)(sum / data.Length), data[0].Definition);
        }
    }
}
