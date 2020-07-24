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
                sum += new BigInteger(sample.Value);
            }
            //TODO: validate bits type and littleendian!
            return new Sample((long)(sum / data.Length), data[0].Bits, data[0].Type, data[0].IsLittleEndian);
        }
    }
}
