using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RICPFitter.Functions
{
    /// <summary>
    /// Fully parametrable gaussian function
    /// </summary>
    public class Gaussian : GenericFittableFunc
    {
        protected Func<double, double, double, double, double, double> func;

        public Gaussian()
        {
            func = new Func<double, double, double, double, double, double>((A, x0, w, y0, x) =>
            {
                return A * Math.Exp(-(x - x0) * (x - x0) / (2 * w * w)) + y0;
            });
            Name = "Gaussian";
            Description = "A * Exp(-(x - x0)² / (2 * w²)) + y0";
            Parameters = new List<FuncParameter>();
            FuncParameter amplitude = new FuncParameter()
            {
                Name = "A",
                Value = 1,
                Description = "Amplitude"
            };
            FuncParameter center = new FuncParameter()
            {
                Name = "x0",
                Value = 0,
                Description = "Center"
            };
            FuncParameter fwhm = new FuncParameter()
            {
                Name = "w",
                Value = 1,
                Description = "FWHM"
            };
            FuncParameter offset = new FuncParameter()
            {
                Name = "y0",
                Value = 0,
                Description = "Offset"
            };
            Parameters.Add(amplitude);
            Parameters.Add(center);
            Parameters.Add(fwhm);
            Parameters.Add(offset);
        }

        public override double DoFit(double[] x, double[] y, List<FuncParameter> initialGuess)
        {
            base.DoFit(x, y, initialGuess);
            var (fitted_A, fitted_x0, fitted_w, fitted_y0) = Fit.Curve(x, y, func,
                initialGuess.Find(p => p.Name == "A").Value,
                initialGuess.Find(p => p.Name == "x0").Value,
                initialGuess.Find(p => p.Name == "w").Value,
                initialGuess.Find(p => p.Name == "y0").Value,
                FitTolerance,
                FitMaxIteration);
            Parameters.Find(p => p.Name == "A").Value = fitted_A;
            Parameters.Find(p => p.Name == "x0").Value = fitted_x0;
            Parameters.Find(p => p.Name == "w").Value = fitted_w;
            Parameters.Find(p => p.Name == "y0").Value = fitted_y0;
            CoeffOfDetermination = GoodnessOfFit.CoefficientOfDetermination(y, GetY(x));
            return CoeffOfDetermination;
        }

        public override (double[], double[]) GenerateData(double start, double end, int nbOfPoints = 41)
        {
            (double[] xData, _) = base.GenerateData(start, end, nbOfPoints);
            return (xData, GetY(xData));
        }

        public override double[] GenerateData(double[] x)
        {
            base.GenerateData(x);
            return GetY(x);
        }

        private double[] GetY(double[] xData)
        {
            double[] yData = new double[xData.Length];
            for (int i = 0; i < xData.Length; i++)
                yData[i] = func(Parameters.Find(p => p.Name == "A").Value,
                                Parameters.Find(p => p.Name == "x0").Value,
                                Parameters.Find(p => p.Name == "w").Value,
                                Parameters.Find(p => p.Name == "y0").Value,
                                xData[i]);

            if (Randomness) AddRandomness(ref yData);

            return yData;
        }
    }
}
