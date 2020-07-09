using System;
using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Math
{
    /// <summary>
    /// https://www.egr.msu.edu/classes/ece480/capstone/fall11/group06/style/Application_Note_ChrisOakley.pdf
    /// </summary>
    public static class FourierTransform
    {
        [return:NotNull]
        public static CartesianCordinate[] Discrete([NotNull] CartesianCordinate[] input)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));

            int n = input.Length;
            var output = new CartesianCordinate[n];
            for (int i = 0; i < n; i++)
            {
                output[i] = new CartesianCordinate();
                for (int j = 0; j < n; j++)
                {
                    var temp = CartesianCordinate.FromPolarCordinate(1, -2 * System.Math.PI * j * i / n);
                    temp *= output[j];
                    output[i] += temp;
                }
            }

            return output;
        }

        [return: NotNull]
        public static CartesianCordinate[] Fast([NotNull] CartesianCordinate[] input)
        {
            _ = input ?? throw new ArgumentNullException(nameof(input));

            int n = input.Length;
            CartesianCordinate[] output = new CartesianCordinate[n];
            CartesianCordinate[] d1, d2, e1, e2;
            if (n == 1)
            {
                output[0] = input[0];
                return output;
            }
            int k;
            e1 = new CartesianCordinate[n / 2];
            d1 = new CartesianCordinate[n / 2];
            for (k = 0; k < n / 2; k++)
            {
                e1[k] = input[2 * k];
                d1[k] = input[(2 * k) + 1];
            }
            d2 = Fast(d1);
            e2 = Fast(e1);
            for (k = 0; k < n / 2; k++)
            {
                d2[k] *= CartesianCordinate.FromPolarCordinate(1, -2 * System.Math.PI * k / n);
            }
            for (k = 0; k < n / 2; k++)
            {
                output[k] = e2[k] + d2[k];
                output[k + (n / 2)] = e2[k] - d2[k];
            }
            return output;
        }
    }
}