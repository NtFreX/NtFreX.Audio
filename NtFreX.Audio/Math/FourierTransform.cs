using System.Diagnostics.CodeAnalysis;

namespace NtFreX.Audio.Math
{
    /// <summary>
    /// https://www.egr.msu.edu/classes/ece480/capstone/fall11/group06/style/Application_Note_ChrisOakley.pdf
    /// </summary>
    public class FourierTransform
    {
        [return:NotNull]
        public static CartesianCordinate[] Discrete([NotNull] CartesianCordinate[] input)
        {
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
            int n = input.Length;
            CartesianCordinate[] output = new CartesianCordinate[n];
            CartesianCordinate[] d, D, e, E;
            if (n == 1)
            {
                output[0] = input[0];
                return output;
            }
            int k;
            e = new CartesianCordinate[n / 2];
            d = new CartesianCordinate[n / 2];
            for (k = 0; k < n / 2; k++)
            {
                e[k] = input[2 * k];
                d[k] = input[2 * k + 1];
            }
            D = Fast(d);
            E = Fast(e);
            for (k = 0; k < n / 2; k++)
            {
                CartesianCordinate temp = CartesianCordinate.FromPolarCordinate(1, -2 * System.Math.PI * k / n);
                D[k] *= temp;
            }
            for (k = 0; k < n / 2; k++)
            {
                output[k] = E[k] + D[k];
                output[k + n / 2] = E[k] - D[k];
            }
            return output;
        }
    }
}