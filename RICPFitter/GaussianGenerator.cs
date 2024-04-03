using MathNet.Numerics.LinearAlgebra.Factorization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    public class GaussianGenerator : IGenerator2D
    {
        private Func<double, double, double, double, double, double> gaussian;
        private double[] xData;
        private double[] yData;

        public GaussianGenerator()
        {
            gaussian = MathFunctions.Gaussian();
        }

        public GaussianGenerator(bool addRandomness, double randomnessStrength = 20) : this()
        {
            AddRandomness = addRandomness;
            RandomnessStrength = randomnessStrength;
        }

        public bool AddRandomness { get; set; } = false;
        public double RandomnessStrength { get; set; } = 20;

        public double XStart { get; set; }
        public double XEnd { get; set; }
        public int NbOfPoints { get; set; } = 0;

        public Dictionary<string, double> FuncParameters { get; } = new Dictionary<string, double>()
        {
            { "amplitude", 1 },
            { "x0", 0 },
            { "fwhm", 1 },
            { "y0", 0 }
        };

        public double[] GenerateData()
        {
            yData = new double[xData.Length];
            for (int i = 0; i < xData.Length; i++)
                yData[i] = gaussian(FuncParameters["amplitude"], FuncParameters["x0"], FuncParameters["fwhm"], FuncParameters["y0"], xData[i]);

            if (AddRandomness)
            {
                Random random = new Random();
                double yMax = yData.Max();
                for (int i = 0; i < yData.Length; i++)
                {
                    double randomVariation = yMax * RandomnessStrength / 100 * random.NextDouble(); // 10% variation
                    yData[i] += randomVariation;
                }
            }
            return yData;
        }

        public double[] GetXData()
        {
            return xData;
        }

        public double[] GetYData()
        {
            return yData;
        }

        public double[] SetXRange()
        {
            if (XStart > XEnd) throw new ArgumentOutOfRangeException("Start value is above end value");
            if (NbOfPoints < 1) throw new ArgumentOutOfRangeException("The number of points must be > 0");

            xData = new double[NbOfPoints];
            double stepSize = (XEnd - XStart) / (NbOfPoints - 1);

            for (int i = 0; i < NbOfPoints; i++)
                xData[i] = XStart + i * stepSize;

            return xData;
        }

        public double[] SetXRange(double start, double end, int nbOfPoints = 41)
        {
            XStart = start;
            XEnd = end;
            NbOfPoints = nbOfPoints;

            return SetXRange();
        }

        #region FOR PROPERTYGRID DISPLAY
        public double Amplitude
        {
            get => FuncParameters["amplitude"];
            set
            {
                FuncParameters["amplitude"] = value;
            }
        }

        public double X0
        {
            get => FuncParameters["x0"];
            set
            {
                FuncParameters["x0"] = value;
            }
        }

        public double FWHM
        {
            get => FuncParameters["fwhm"];
            set
            {
                FuncParameters["fwhm"] = value;
            }
        }

        public double Y0
        {
            get => FuncParameters["y0"];
            set
            {
                FuncParameters["y0"] = value;
            }
        }
        #endregion
    }
}
