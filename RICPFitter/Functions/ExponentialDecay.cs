using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter.Functions
{
    public class ExponentialDecay : GenericFittableFunc
    {
        protected Func<double, double, double, double, double> func;

        public ExponentialDecay()
        {
            func = new Func<double, double, double, double, double>((A, τ, y0, t) =>
            {
                return A * Math.Exp(- t / τ) + y0;
            });
            Name = "Exponantial decay";
            Equation = "A * Exp(- t / τ) + y0";
            Parameters = [];
            FuncParameter amplitude = new()
            {
                Name = "A",
                Value = 1,
                Description = "amplitude"
            };
            FuncParameter lifetime = new()
            {
                Name = "τ",
                Value = 1,
                Description = "lifetime",
                Unit = "s"
            };
            FuncParameter offset = new()
            {
                Name = "y0",
                Value = 0,
                Description = "offset"
            };
        }

        public override double DoFit(double[] x, double[] y, List<FuncParameter> initialGuess)
        {
            base.DoFit(x, y, initialGuess);
            var (fitted_A, fitted_τ, fitted_y0) = Fit.Curve(x, y, func,
                initialGuess.Find(p=>p.Name == "A").Value,
                initialGuess.Find(p => p.Name == "τ").Value,
                initialGuess.Find(p => p.Name == "y0").Value,
                FitTolerance,
                FitMaxIteration);
            Parameters.Find(p => p.Name == "A").Value = fitted_A;
            Parameters.Find(p => p.Name == "τ").Value = fitted_τ;
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
                                Parameters.Find(p => p.Name == "τ").Value,
                                Parameters.Find(p => p.Name == "y0").Value,
                                xData[i]);

            if (Randomness) AddRandomness(ref yData);

            return yData;
        }
    }
}
