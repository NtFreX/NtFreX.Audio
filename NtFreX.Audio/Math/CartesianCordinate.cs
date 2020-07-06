﻿using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Math
{
    public class CartesianCordinate
    {
        public double X { [return:MaybeNull] get; }
        public double Y { [return: MaybeNull] get; }

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

        [return:NotNull] public static CartesianCordinate FromPolarCordinate(double r, double radians)
            => new CartesianCordinate(r * System.Math.Cos(radians), r * System.Math.Sin(radians));

        [return: NotNull] public static CartesianCordinate operator +([MaybeNull] CartesianCordinate a, [MaybeNull] CartesianCordinate b)
            => new CartesianCordinate((a?.X ?? 0d) + (b?.X ?? 0d), (a?.Y ?? 0d) + (b?.Y ?? 0d));

        [return: NotNull] public static CartesianCordinate operator -([MaybeNull] CartesianCordinate a, [MaybeNull] CartesianCordinate b)
            => new CartesianCordinate((a?.X ?? 0d) - (b?.X ?? 0d), (a?.Y ?? 0d) - (b?.Y ?? 0d));

        [return: NotNull] public static CartesianCordinate operator *([MaybeNull] CartesianCordinate a, [MaybeNull] CartesianCordinate b)
            => new CartesianCordinate(((a?.X ?? 0d) * (b?.X ?? 0d)) - ((a?.Y ?? 0d) * (b?.Y ?? 0d)), ((a?.X ?? 0d) * (b?.Y ?? 0d)) + ((a?.Y ?? 0d) * (b?.X ?? 0d)));

        [return: NotNull] public override string ToString()
        {
            return $"x{X}:y{Y}";
        }
    }
}