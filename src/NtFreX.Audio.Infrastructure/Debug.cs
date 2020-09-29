using System;
using System.Runtime.CompilerServices;

namespace NtFreX.Audio.Infrastructure
{
    public static class Debug
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1801 // Review unused parameters
        public static void Assert(bool condition, string message)
#pragma warning restore CA1801 // Review unused parameters
        {
#if DEBUG
            // fun fact: System.Debug.Assert is so slow it cannot be used here
            if (!condition)
            {
                throw new Exception(message);
            }
#endif
        }
    }
}
