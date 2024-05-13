using MathNet.Numerics;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RICPFitter.Functions
{
    public class ExternalFunc : GenericFittableFunc
    {
        private object function;

        public int MaxNbOfParameters { get; private set; } = 5;

        public ExternalFunc() { }

        public static ExternalFunc FromXml(XmlNode node)
        {
            ExternalFunc result = new ExternalFunc();
            result.Name = node.Attributes["name"].Value;
            List<FuncParameter> parameters = new List<FuncParameter>();
            string variableName = "";
            bool singleVariableFound = false;
            string equation = "";
            bool equationFound = false;
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.Name == "Parameter")
                {
                    double initValue = Double.Parse(subnode.Attributes["defaultValue"].InnerText,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture);
                    FuncParameter parameter = new FuncParameter()
                    {
                        Name = subnode.InnerText,
                        Value = initValue,
                        Description = subnode.Attributes["description"].InnerText,
                    };
                    if (subnode.Attributes["unit"] != null) parameter.Unit = subnode.Attributes["unit"].InnerText;
                    parameters.Add(parameter);
                }
                else if (subnode.Name == "Variable")
                {
                    if (!singleVariableFound)
                    {
                        variableName = subnode.InnerText;
                        singleVariableFound = true;
                    }
                    else // To much variables
                    {
                        throw new ArgumentException("A function must contain only 1 variable");
                    }
                }
                else if (subnode.Name == "Equation")
                {
                    if (!equationFound)
                    {
                        equation = subnode.InnerText;
                        result.Description = equation;
                        equationFound = true;
                    }
                    else // To much equation definition
                    {
                        throw new ArgumentException("A function must contain only 1 equation");
                    }
                }
            }
            if (!singleVariableFound) throw new ArgumentException("A function must contain 1 variable");
            if (parameters.Count < 1) throw new ArgumentException("A function must contain at least one parameter");
            if (parameters.Count > result.MaxNbOfParameters) throw new ArgumentException($"A function must contain at max {result.MaxNbOfParameters} parameters");
            if (!equationFound) throw new ArgumentException("A function must contain 1 equation");
            if (!equation.Contains(variableName)) throw new ArgumentException($"Variable {variableName} is not included in the equation {equation}");

            
            result.GenerateFunction(parameters, variableName, equation);

            result.Parameters = parameters;

            return result;
        }

        private void GenerateFunction(List<FuncParameter> parameters, string variable, string equation)
        {
            string code = "return new Func<";
            for (int i = 0; i < parameters.Count; i++) code += "double, ";
            code += "double, double>((";
            foreach (FuncParameter subparam in parameters)
            {
                if (!equation.Contains(subparam.Name)) throw new ArgumentException($"Parameter {subparam.Name} is not included in the equation {equation}");
                code += subparam.Name + ", ";
            }
            code += variable + ") => " + equation + ");";

            var scriptOptions = ScriptOptions.Default
                .WithReferences(typeof(double).Assembly) // Include the necessary assembly references
                .WithImports("System"); // Include the necessary namespaces

            // Evaluate the code using CSharpScript
            var script = CSharpScript.Create<object>(code, scriptOptions);
            var result = script.RunAsync().Result;
            function = result.ReturnValue;
            if (function == null) throw new NullReferenceException($"Failed to generate the function {equation}");
        }

        public override double DoFit(double[] x, double[] y, List<FuncParameter> initialGuess)
        {
            base.DoFit(x, y, initialGuess);

            string code = "var fittedParam = Fit.Curve(x, y,(Func<";
            int nbOfParam = Parameters.Count;
            for (int i = 0; i < nbOfParam; i++)
            {
                code += "double, ";
            }
            code += "double>)function,";
            for (int i = 0; i < nbOfParam; i++)
            {
                double value = initialGuess[i].Value;
                code += value.ToString() + ",";
            }
            code += $"{FitTolerance}, {FitMaxIteration});";
            code += "return (ITuple)fittedParam;";

            var scriptOptions = ScriptOptions.Default
                .WithReferences(typeof(double).Assembly)
                .WithImports(new string[2] {"System", "MathNet.Numerics" });
            var script = CSharpScript.Create<ITuple>(code, scriptOptions, globalsType: typeof(ExternalFunc));
            var result = script.RunAsync(this).Result.ReturnValue;
            for (int i = 0; i < result.Length; i++)
            {
                Parameters[i].Value = (double)result[i];
            }

            CoeffOfDetermination = GoodnessOfFit.CoefficientOfDetermination(y, GetY(x));
            return CoeffOfDetermination;
        }

        private double[] GetY(double[] xData)
        {
            double[] yData = new double[xData.Length];

            // Base code
            int nbOfParam = Parameters.Count;
            string paramAsStr = "";
            for (int i = 0; i < nbOfParam; i++)
            {
                paramAsStr += Parameters[i].Value.ToString() + ",";
            }

            // Scripting options
            string baseCode = "return function(" + paramAsStr;
            var scriptOptions = ScriptOptions.Default
                .WithReferences(typeof(double).Assembly)
                .WithImports("System");

            // Run code to get yData
            for (int i = 0; i < xData.Length; i++)
            {
                string code = baseCode + xData[i].ToString() + ");";
                var script = CSharpScript.Create<double>(code, scriptOptions, globalsType: typeof(ExternalFunc));
                yData[i] = script.RunAsync(this).Result.ReturnValue;
            }

            if (Randomness) AddRandomness(ref yData);

            return yData;
        }
    }
}
