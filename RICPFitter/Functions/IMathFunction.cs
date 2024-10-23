using System;
using System.Collections.Generic;

namespace RICPFitter.Functions
{
    /// <summary>
    /// General definition of a mathematic function
    /// </summary>
    public interface IMathFunction
    {
        /// <summary>
        /// Name of the function
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description e.g. formula
        /// </summary>
        string Equation { get; }

        /// <summary>
        /// Category of the function (for sorting purpose)
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Name of the variable
        /// </summary>
        string VariableName { get; }

        /// <summary>
        /// Function parameters
        /// </summary>
        List<FuncParameter> Parameters { get; set; }
    }
}
