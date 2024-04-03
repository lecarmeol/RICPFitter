using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    /// <summary>
    /// A class to fit data with a fully described gaussian
    /// See RICPFitter.MathFunctions.Gaussian
    /// </summary>
    public class GaussianFitter : GenericFitter2D
    {
        private Func<double, double, double, double, double, double> gaussian;

        /// <summary>
        /// New fitter with inital guesses
        /// </summary>
        /// <param name="amplitude"></param>
        /// <param name="x0"></param>
        /// <param name="fwhm"></param>
        /// <param name="y0"></param>
        public GaussianFitter(double amplitude = 1, double x0 = 0, double fwhm = 1, double y0 = 0)
        {
            gaussian = MathFunctions.Gaussian();
            InitialParameters = new Dictionary<string, double>()
            {
                { "amplitude", amplitude },
                { "x0", x0 },
                { "fwhm", fwhm },
                { "y0", y0 }
            };
            FittedParameters = new Dictionary<string, double>(InitialParameters);
        }

        /// <summary>
        /// New default fitter
        /// </summary>
        public GaussianFitter(): this(1, 0, 1, 0) { }

        /// <inheritdoc/>
        public override double DoFit(double tolerance = 1E-08, int maxIterations = 1000)
        {
            var (fitted_A, fitted_x0, fitted_fwhm, fitted_y0) = Fit.Curve(rawX, rawY, gaussian, 
                InitialParameters["amplitude"], InitialParameters["x0"], InitialParameters["fwhm"], InitialParameters["y0"], tolerance, maxIterations);
            FittedParameters["amplitude"] = fitted_A;
            FittedParameters["x0"] = fitted_x0;
            FittedParameters["fwhm"] = fitted_fwhm;
            FittedParameters["y0"] = fitted_y0;
            fittedY = new double[rawY.Length];
            for (int i = 0; i < fittedY.Length; i++)
            {
                fittedY[i] = gaussian(fitted_A, fitted_x0, fitted_fwhm, fitted_y0, rawX[i]);
            }
            CoeffOfDetermination = GoodnessOfFit.CoefficientOfDetermination(rawY, fittedY);
            return CoeffOfDetermination;
        }

        #region FOR PROPERTYGRID DISPLAY
        public double InitAmplitude
        {
            get => InitialParameters["amplitude"];
            set
            {
                InitialParameters["amplitude"] = value;
            }
        }

        public double InitX0
        {
            get => InitialParameters["x0"];
            set
            {
                InitialParameters["x0"] = value;
            }
        }

        public double InitFWHM
        {
            get => InitialParameters["fwhm"];
            set
            {
                InitialParameters["fwhm"] = value;
            }
        }

        public double InitY0
        {
            get => InitialParameters["y0"];
            set
            {
                InitialParameters["y0"] = value;
            }
        }
        #endregion
    }
}
