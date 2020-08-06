﻿using System.Linq;

namespace NtFreX.Audio.Console
{
    /// <summary>
    /// Simple progressbar which logs to the current line.
    /// This type is not thread safe.
    /// </summary>
    internal static class ConsoleProgressBar
    {
        private const int MaxLength = 40;
        private const int StartAtFromLeft = 2;

        private static double lastProgress = 0;

        public static void LogProgress(double progress)
        {
            var diff = System.Math.Abs(progress - lastProgress);
            if (diff > 0.01 || progress == 0 || progress == 1)
            {
                System.Console.CursorLeft = StartAtFromLeft;
                System.Console.Write("<" + string.Join(string.Empty, Enumerable.Repeat("█", (int)(MaxLength * progress))));
                System.Console.CursorLeft = MaxLength + 1 + StartAtFromLeft;
                System.Console.Write(">");
                lastProgress = progress;
            }
        }
    }
}
