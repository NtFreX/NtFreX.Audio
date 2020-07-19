using NtFreX.Audio.Containers;
using NtFreX.Audio.Extensions;
using NtFreX.Audio.Infrastructure;
using NtFreX.Audio.Math;
using NtFreX.Audio.Samplers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtFreX.Audio.Sampler.Console
{

    internal static class ConsoleHelper
    {
        static double lastProgress = 0;
        const int length = 40;
        const int left = 2;

        public static void LogProgress(double progress)
        {
            var diff = System.Math.Abs(progress - lastProgress);
            if (diff > 0.01 || progress == 0 || progress == 1)
            {
                System.Console.CursorLeft = left;
                System.Console.Write("<" + string.Join(string.Empty, Enumerable.Repeat("█", (int)(length * progress))));
                System.Console.CursorLeft = length + 1 + left;
                System.Console.Write(">");
                lastProgress = progress;
            }
        }
    }
}
