using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter.Functions
{
    /// <summary>
    /// Description of a fittable function
    /// </summary>
    public interface IFittable : IGenerable
    {
        /// <summary>
        /// Coefficient of deterimnation of the fit
        /// </summary>
        double CoeffOfDetermination { get; }

        /// <summary>
        /// Fit tolerance
        /// </summary>
        double FitTolerance { get; set; }

        /// <summary>
        /// Fit max iteration
        /// </summary>
        int FitMaxIteration { get; set; }

        /// <summary>
        /// Initial guess values
        /// </summary>
        List<FuncParameter> GuessParameters { get; }

        /// <summary>
        /// Perform the fit
        /// </summary>
        /// <param name="x">x array</param>
        /// <param name="y">y array</param>
        /// <returns>Coeff of determination</returns>
        double DoFit(double[] x, double[] y);

        /// <summary>
        /// Perform the fit
        /// </summary>
        /// <param name="x">x array</param>
        /// <param name="y">y array</param>
        /// <param name="initialGuess">Initial guess</param>
        /// <returns>Coeff of determination</returns>
        double DoFit(double[] x, double[] y, List<FuncParameter> initialGuess);

        /// <summary>
        /// Fired when the a fit is performed
        /// </summary>
        event Action<IFittable, double[], double[]> FitPerformed;
    }
}
