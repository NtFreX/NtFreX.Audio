namespace NtFreX.Audio.Math
{
    public class CartesianCordinate
    {
        public double X { get; }
        public double Y { get; }

        public CartesianCordinate()
        {
            X = 0.0d;
            Y = 0.0d;
        }

        public CartesianCordinate(double x)
        {
            X = x;
            Y = 0.0d;
        }

        public CartesianCordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static CartesianCordinate FromPolarCordinate(double r, double radians)
            => new CartesianCordinate(r * System.Math.Cos(radians), r * System.Math.Sin(radians));

        public static CartesianCordinate operator +(CartesianCordinate a, CartesianCordinate b)
            => new CartesianCordinate((a?.X ?? 0d) + (b?.X ?? 0d), (a?.Y ?? 0d) + (b?.Y ?? 0d));

        public static CartesianCordinate operator -(CartesianCordinate a, CartesianCordinate b)
            => new CartesianCordinate((a?.X ?? 0d) - (b?.X ?? 0d), (a?.Y ?? 0d) - (b?.Y ?? 0d));

        public static CartesianCordinate operator *(CartesianCordinate a, CartesianCordinate b)
            => new CartesianCordinate(((a?.X ?? 0d) * (b?.X ?? 0d)) - ((a?.Y ?? 0d) * (b?.Y ?? 0d)), ((a?.X ?? 0d) * (b?.Y ?? 0d)) + ((a?.Y ?? 0d) * (b?.X ?? 0d)));

        public override string ToString()
        {
            return $"x{X}:y{Y}";
        }
    }
}