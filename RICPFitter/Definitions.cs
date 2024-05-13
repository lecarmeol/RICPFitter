using RICPFitter.Functions;
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
        /// List of available IMathFunction class names
        /// </summary>
        public static List<string> FunctionNames => new List<string>() { "Gaussian", "Lorentzian", "ExponentialDecay" };

        /// <summary>
        /// List of available IMathFunction class types
        /// </summary>
        public static List<string> FunctionTypes
        {
            get
            {
                string namespaceAsStr = typeof(Gaussian).Namespace;
                string prefix = namespaceAsStr + ".";
                string suffix = "," + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                List<string> result = new List<string>();
                foreach(string str in FunctionNames)
                {
                    result.Add(prefix + str + suffix);
                }
                return result;
            }
        }
    }
}
