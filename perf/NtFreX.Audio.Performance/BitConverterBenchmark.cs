using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Runtime.CompilerServices;

namespace NtFreX.Audio.Performance
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [SimpleJob(RuntimeMoniker.CoreRt31)]
    [SimpleJob(RuntimeMoniker.CoreRt50)]
    public class BitConverterBenchmark
    {
        private readonly byte[] data = new byte[] { 2, 8 };

        [Benchmark]
        public unsafe short ToInt16Unsave()
        {
            fixed (byte* ptr = &data[0])
            {
                return *(short*)ptr;
            }
        }
        [Benchmark]
        public short ToInt16Save()
        {
            return BitConverter.ToInt16(data);
        }
        [Benchmark]
        public short ToInt16Managed()
        {
            return Unsafe.ReadUnaligned<short>(ref data[0]);
        }
    }
}
