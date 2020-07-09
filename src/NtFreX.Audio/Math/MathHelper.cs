namespace NtFreX.Audio.Math
{
    public static class MathHelper
    {
        public static long LimitTo(long max, long min, long value)
            => value < 0 ? System.Math.Max(min, value) : System.Math.Min(max, value);
    }
}
