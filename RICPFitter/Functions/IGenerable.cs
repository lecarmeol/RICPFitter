using System;

namespace RICPFitter.Functions
{
    /// <summary>
    /// Description of generable function from any X data
    /// </summary>
    public interface IGenerable : IMathFunction
    {
        /// <summary>
        /// Add randomness in generated data
        /// </summary>
        bool Randomness { get; set; }

        /// <summary>
        /// Randomness Strength in %
        /// </summary>
        double RandomnessStrength { get; set; }

        /// <summary>
        /// Generate y data from a series of regularly spaced x values
        /// </summary>
        /// <param name="start">start value</param>
        /// <param name="end">end value</param>
        /// <param name="nbOfPoints">number of points</param>
        /// <returns>X and Y array</returns>
        (double[], double[]) GenerateData(double start, double end, int nbOfPoints = 41);

        /// <summary>
        /// Generate y data from an array of x value
        /// </summary>
        /// <param name="x">x array</param>
        /// <returns></returns>
        double[] GenerateData(double[] x);
    }
}
