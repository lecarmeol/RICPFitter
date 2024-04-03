using System.Collections.Generic;

namespace RICPFitter
{
    public interface IFitter2D
    {
        /// <summary>
        /// Initial guess parameters
        /// </summary>
        Dictionary<string, double> InitialParameters { get; }

        /// <summary>
        /// Fitted parameters
        /// </summary>
        Dictionary<string, double> FittedParameters { get; }

        /// <summary>
        /// Coefficient of deterimnation of the fit
        /// </summary>
        double CoeffOfDetermination { get; set; }

        /// <summary>
        /// Perform the fit
        /// </summary>
        /// <param name="tolerance">Tolerance</param>
        /// <param name="maxIterations">Max nb of iterations</param>
        /// <returns>Coeff of determination</returns>
        double DoFit(double tolerance = 1E-08, int maxIterations = 1000);

        /// <summary>
        /// Get the x data
        /// </summary>
        /// <returns></returns>
        double[] GetXData();

        /// <summary>
        /// Get the raw y data
        /// </summary>
        /// <returns></returns>
        double[] GetRawData();

        /// <summary>
        /// Get the fitted y data
        /// </summary>
        /// <returns></returns>
        double[] GetFittedData();

        /// <summary>
        /// Set raw data
        /// </summary>
        /// <param name="x">x array</param>
        /// <param name="y">y array</param>
        void SetRawData(double[] x, double[] y);
    }
}