using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    public abstract class GenericFitter2D : IFitter2D
    {
        protected double[] rawX;
        protected double[] rawY;
        protected double[] fittedY;

        /// <inheritdoc/>
        public double CoeffOfDetermination { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, double> InitialParameters { get; protected set; }

        /// <inheritdoc/>
        public Dictionary<string, double> FittedParameters { get; protected set; }

        /// <inheritdoc/>
        public void SetRawData(double[] x, double[] y)
        {
            rawX = x;
            rawY = y;
        }

        /// <inheritdoc/>
        public virtual double DoFit(double tolerance = 1E-08, int maxIterations = 1000)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public double[] GetFittedData()
        {
            return fittedY;
        }

        /// <inheritdoc/>
        public double[] GetXData()
        {
            return rawX;
        }

        /// <inheritdoc/>
        public double[] GetRawData()
        {
            return rawY;
        }
    }
}
