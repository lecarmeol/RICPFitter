using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter.Functions
{
    internal class Lorentzian : Gaussian
    {
        public Lorentzian() : base()
        {
            func = new Func<double, double, double, double, double, double>((A, x0, w, y0, x) =>
            {
                return A / (1 + Math.Pow(2 * (x - x0) / w, 2)) + y0;
            });
            Name = "Lorentzian";
            Description = "A / (1 + (2 * (x-x0) / w)²) + y0";
        }
    }
}
