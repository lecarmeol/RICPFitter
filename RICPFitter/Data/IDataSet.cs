using System;

namespace RICPFitter.Data
{
    /// <summary>
    /// Interface describing a set of XY data
    /// </summary>
    public interface IDataSet
    {
        /// <summary>
        /// Title
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// X data
        /// </summary>
        double[] XData { get; set; }

        /// <summary>
        /// X label or description
        /// </summary>
        string XLabel { get; set; }

        /// <summary>
        /// Y data
        /// </summary>
        double[] YData { get; set; }

        /// <summary>
        /// Y label or description
        /// </summary>
        string YLabel { get; set; }

        /// <summary>
        /// Fired when X or Y data change
        /// </summary>
        event Action<DataSet> InputDataChanged;
    }
}