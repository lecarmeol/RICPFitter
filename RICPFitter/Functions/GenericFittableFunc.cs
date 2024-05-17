using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter.Functions
{
    public class GenericFittableFunc : IFittable
    {
        [DisplayName("Name")]
        [Description("Name")]
        [Category("1) General")]
        public string Name { get; protected set; } = "Generic";

        [DisplayName("Formula")]
        [Description("Formula")]
        [Category("1) General")]
        public string Description { get; protected set; } = string.Empty;

        [DisplayName("Category")]
        [Description("Category")]
        [Category("1) General")]
        public string Category { get; set; } = "Other";

        [DisplayName("Parameters")]
        [Description("Parameters of the function")]
        [Category("1) General")]
        public List<FuncParameter> Parameters { get; set; }

        [DisplayName("Coeff of Det.")]
        [Description("Coefficient of determination (~R²)")]
        [Category("3) Fit")]
        public double CoeffOfDetermination { get; protected set; }

        [DisplayName("Add randomness")]
        [Description("Add randomness to function generation")]
        [Category("2) Generation")]
        public bool Randomness { get; set; } = false;

        [DisplayName("Randomness strength (%)")]
        [Description("Randomness strength (%)")]
        [Category("2) Generation")]
        public double RandomnessStrength { get; set; } = 10;

        [DisplayName("Fit tolerance")]
        [Description("Fit tolerance (least square method)")]
        [Category("3) Fit")]
        public double FitTolerance { get; set; } = 1e-8;

        [DisplayName("Fit max iteration")]
        [Description("Fit max number of iteration (least square method)")]
        [Category("3) Fit")]
        public int FitMaxIteration { get; set; } = 1000;

        [DisplayName("Initial guess")]
        [Description("Initial guess of fit")]
        [Category("3) Fit")]
        public List<FuncParameter> GuessParameters { get; protected set; }

        public event Action<IFittable, double[], double[]> FitPerformed;
        protected void OnFitPerformed(IFittable function, double[] x, double[] y)
        {
            FitPerformed?.Invoke(function, x, y);
        }


        public virtual double DoFit(double[] x, double[] y)
        {
            return DoFit(x, y, GuessParameters);
        }

        public virtual double DoFit(double[] x, double[] y, List<FuncParameter> initialGuess)
        {
            if (x.Length != y.Length) 
            {
                throw new ArgumentException("X and Y array must have the same size");
            }
            if (initialGuess.Intersect(Parameters).Count() != initialGuess.Count)
            {
                throw new ArgumentException("Parameters of the inital guess are different from the ones expected");
            }
            return 0;
        }

        public virtual (double[], double[]) GenerateData(double start, double end, int nbOfPoints = 41)
        {
            if (start > end) throw new ArgumentException("Start value must be higher than end value");
            ArgumentOutOfRangeException.ThrowIfLessThan(nbOfPoints, 1);

            double[] xData = new double[nbOfPoints];
            double stepSize = (end - start) / (nbOfPoints - 1);

            for (int i = 0; i < nbOfPoints; i++)
                xData[i] = start + i * stepSize;

            return (xData, null);
        }

        public virtual double[] GenerateData(double[] x)
        {
            return null;
        }

        /// <summary>
        /// Add randomness to a 1D array
        /// </summary>
        /// <param name="yData"></param>
        protected void AddRandomness(ref double[] yData)
        {
            Random random = new();
            double yMax = yData.Max();
            for (int i = 0; i < yData.Length; i++)
            {
                double randomVariation = yMax * RandomnessStrength / 100 * random.NextDouble();
                yData[i] += randomVariation;
            }
        }
    }
}
