using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NtFreX.Audio.Infrastructure
{
    public static class SampleExtensions
    {
        public static Sample Average(this IEnumerable<Sample> samples)
        {
            //TODO: support float
            var sum = new BigInteger(0);
            var data = samples.ToArray();
            foreach(var sample in data)
            {
                if(data[0].Definition != sample.Definition)
                {
                    throw new Exception("The given samples are not of the same type");
                }
                sum += new BigInteger(sample.Value);
            }
            return new Sample((long)(sum / data.Length), data[0].Definition);
        }
    }
}
