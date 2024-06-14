using RICPFitter.Functions;
using System;
using System.Collections.Generic;

namespace RICPFitter.Collections
{
    /// <summary>
    /// Description of a function collection with custom list features (to be extended)
    /// </summary>
    public interface IFunctionCollection : ICloneable
    {
        /// <summary>
        /// All functions
        /// </summary>
        List<IFittable> Functions { get; }

        /// <summary>
        /// Find a single function
        /// </summary>
        /// <param name="name">function name</param>
        /// <param name="category">optional function category</param>
        /// <returns></returns>
        IFittable FindByName(string name, string category = null);

        /// <summary>
        /// Sort functions by category and name
        /// </summary>
        void SortByCategoryAndName();
    }
}
