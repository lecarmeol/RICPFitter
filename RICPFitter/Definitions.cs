using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    /// <summary>
    /// Contain static definitions
    /// </summary>
    public static class Definitions
    {
        /// <summary>
        /// List of available fitter class names
        /// </summary>
        public static List<string> FitterNames => new List<string>() { "GaussianFitter", "NormalizedDistribFitter" };

        /// <summary>
        /// List of available fitter class types
        /// </summary>
        public static List<string> FitterTypes
        {
            get
            {
                string namespaceAsStr = typeof(GaussianFitter).Namespace;
                string prefix = namespaceAsStr + ".";
                string suffix = "," + namespaceAsStr;
                List<string> result = new List<string>();
                foreach(string str in FitterNames)
                {
                    result.Add(prefix + str + suffix);
                }
                return result;
            }
        }
    }
}
