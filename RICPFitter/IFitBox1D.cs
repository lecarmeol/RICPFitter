using RICPFitter.Collections;
using RICPFitter.Data;
using RICPFitter.Functions;
using System;

namespace RICPFitter
{
    public interface IFitBox1D
    {
        /// <summary>
        /// Fit function
        /// </summary>
        IFittable FitFunction { get; set; }

        /// <summary>
        /// Input data
        /// </summary>
        IDataSet InputData { get; set; }

        /// <summary>
        /// List of available fit functions
        /// </summary>
        IFunctionCollection ListOfFitFunctions { get; set; }

        /// <summary>
        /// Fired when fit data are changed
        /// </summary>
        event Action<IFittable, double[], double[]> FitDataChanged;

        /// <summary>
        /// Fired when the fit function type is changed
        /// </summary>
        event Action<IFittable> FitFunctionChanged;

        /// <summary>
        /// Fired when input data are changed
        /// </summary>
        event Action<double[], double[]> InputDataChanged;
    }
}