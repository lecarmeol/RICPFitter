using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    public interface IGenerator2D
    {
        /// <summary>
        /// Initial guess parameters
        /// </summary>
        Dictionary<string, double> FuncParameters { get; }

        /// <summary>
        /// Add randomness in generated data
        /// </summary>
        bool AddRandomness {  get; set; }

        /// <summary>
        /// Randomness Strength
        /// </summary>
        double RandomnessStrength { get; set; }

        /// <summary>
        /// X start value
        /// </summary>
        double XStart { get; set; }

        /// <summary>
        /// Y start value
        /// </summary>
        double XEnd { get; set; }

        /// <summary>
        /// Number of points
        /// </summary>
        int NbOfPoints { get; set; }

        /// <summary>
        /// Set X range from internal values
        /// </summary>
        /// <returns></returns>
        double[] SetXRange();

        /// <summary>
        /// Set the X data
        /// </summary>
        /// <param name="start">start value</param>
        /// <param name="end">end value</param>
        /// <param name="nbOfPoints">number of points</param>
        /// <returns></returns>
        double[] SetXRange(double start, double end, int nbOfPoints = 41);

        /// <summary>
        /// Generate the data of the function
        /// </summary>
        /// <returns></returns>
        double[] GenerateData();

        /// <summary>
        /// Get X data
        /// </summary>
        /// <returns></returns>
        double[] GetXData();

        /// <summary>
        /// Get Y data
        /// </summary>
        /// <returns></returns>
        double[] GetYData();
    }
}
