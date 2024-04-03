using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    /// <summary>
    /// Definition of standard math functions
    /// </summary>
    public static class MathFunctions
    {
        /// <summary>
        /// Gaussian 2D f(x) = 1/(σ*sqrt(2*pi)) * exp(-1/2*(x-µ)²/σ²)<br/>
        /// Parameter order: <br/>
        ///     Variance square root σ<br/>
        ///     Mean µ<br/>
        /// </summary>
        /// <returns></returns>
        public static Func<double, double, double, double> GaussianNorm()
        {
            return new Func<double, double, double, double>((σ, μ, x) =>
            {
                return MathNet.Numerics.Distributions.Normal.PDF(μ, σ, x);
            });
        }

        /// <summary>
        /// Gaussian 2D f(x) = A * exp((x-x0)²/2w²) + y0<br/>
        /// Parameters order: <br/>
        ///     Amplitude A<br/>
        ///     Center x0<br/>
        ///     FWHM w<br/>
        ///     Offset y0<br/>
        ///     position x
        /// </summary>
        /// <returns>y</returns>
        public static Func<double, double, double, double, double, double> Gaussian()
        {
            return new Func<double, double, double, double, double, double>((A, x0, w, y0, x) =>
            {
                return A * Math.Exp(-(x - x0) * (x - x0) / (2 * w * w)) + y0;
            });
        }
    }
}
