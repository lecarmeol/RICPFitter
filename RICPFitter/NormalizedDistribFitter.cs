using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    /// <summary>
    /// A class to fit data with a normalized gaussian distribution</br>
    /// See RICPFitter.MathFunctions.GaussianNorm
    /// </summary>
    public class NormalizedDistribFitter : GenericFitter2D
    {
        private Func<double, double, double, double> gaussian;

        /// <summary>
        /// New fitter with inital guesses
        /// </summary>
        /// <param name="variance">Variance square root</param>
        /// <param name="mean">Mean value (center)</param>
        public NormalizedDistribFitter(double variance = 1, double mean = 0)
        {
            gaussian = MathFunctions.GaussianNorm();
            InitialParameters = new Dictionary<string, double>()
            {
                { "variance", variance },
                { "mean", mean }
            };
            FittedParameters = new Dictionary<string, double>(InitialParameters);
        }

        /// <summary>
        /// New default fitter
        /// </summary>
        public NormalizedDistribFitter(): this(1, 0) { }

        /// <inheritdoc/>
        public override double DoFit(double tolerance = 1E-08, int maxIterations = 1000)
        {
            var (fitted_sigma, fitted_mean) = Fit.Curve(rawX, rawY, gaussian,
                InitialParameters["variance"], InitialParameters["mean"], tolerance, maxIterations);
            FittedParameters["variance"] = fitted_sigma;
            FittedParameters["mean"] = fitted_mean;
            fittedY = new double[rawY.Length];
            for (int i = 0; i < fittedY.Length; i++)
            {
                fittedY[i] = gaussian(fitted_mean, fitted_mean, rawX[i]);
            }
            CoeffOfDetermination = GoodnessOfFit.CoefficientOfDetermination(rawY, fittedY);
            return CoeffOfDetermination;
        }

        #region FOR PROPERTYGRID DISPLAY
        public double InitVariance
        {
            get => InitialParameters["variance"];
            set
            {
                InitialParameters["variance"] = value;
            }
        }

        public double InitMean
        {
            get => InitialParameters["mean"];
            set
            {
                InitialParameters["mean"] = value;
            }
        }
        #endregion

    }
}
