namespace NtFreX.Audio.Math
{
    public static class MathHelper
    {
        public static long LimitTo(long max, long min, long value)
            => value < 0 ? System.Math.Max(min, value) : System.Math.Min(max, value);

        public static ulong GreatestCommonDivisor(ulong a, ulong b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }

            return a == 0 ? b : a;
        }
    }
}
