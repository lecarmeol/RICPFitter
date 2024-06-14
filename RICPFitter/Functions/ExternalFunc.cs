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
            ExternalFunc result = new()
            {
                Name = node.Attributes["name"].Value
            };
            string cat = node.Attributes["category"].Value;
            if (cat != null)
            {
                result.Category = cat;
            }
            List<FuncParameter> parameters = [];
            bool singleVariableFound = false;
            bool equationFound = false;
            foreach (XmlNode subnode in node.ChildNodes)
            {
                if (subnode.Name == "Parameter")
                {
                    double initValue = Double.Parse(subnode.Attributes["defaultValue"].InnerText,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture);
                    FuncParameter parameter = new()
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
                        result.VariableName = subnode.InnerText;
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
                        result.Description = subnode.InnerText;
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
            if (!result.Description.Contains(result.VariableName)) throw new ArgumentException($"Variable {result.VariableName} is not included in the equation {result.Description}");

            
            result.GenerateFunction(parameters, result.VariableName, result.Description);

            result.Parameters = parameters;
            result.GuessParameters = new List<FuncParameter>(parameters);

            return result;
        }

        public void ToXml(XmlNode parentNode)
        {
            XmlElement funcNode = parentNode.OwnerDocument.CreateElement("Function");
            funcNode.SetAttribute("name", Name);
            funcNode.SetAttribute("category", Category);
            foreach (FuncParameter parameter in Parameters)
            {
                XmlElement paramNode = funcNode.OwnerDocument.CreateElement("Parameter");
                paramNode.SetAttribute("description", parameter.Description);
                paramNode.SetAttribute("defaultValue", parameter.Value.ToString().Replace(',','.'));
                if (parameter.Unit != null)
                {
                    paramNode.SetAttribute("unit", parameter.Unit);
                }
                paramNode.InnerText = parameter.Name;
                funcNode.AppendChild(paramNode);
            }
            XmlElement variableNode = funcNode.OwnerDocument.CreateElement("Variable");
            variableNode.InnerText = VariableName;
            funcNode.AppendChild(variableNode);
            XmlElement equationNode = funcNode.OwnerDocument.CreateElement("Equation");
            equationNode.InnerText = Description;
            funcNode.AppendChild(equationNode);

            parentNode.AppendChild(funcNode);
        }

        /// <summary>
        /// Generate a function (delegate) from an equation as string<br/>
        /// Equation is transformed into function via runtime C# scripting
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="variable"></param>
        /// <param name="equation"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
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

            List<object> rawFitParam = [];
            switch (function)
            {
                case Func<double, double, double> func1Param:
                    rawFitParam.Add(Fit.Curve(x, y, func1Param, initialGuess[0].Value, FitTolerance, FitMaxIteration));
                    break;
                case Func<double, double, double, double> func2Param:
                    var fitParam = Fit.Curve(x, y, 
                        func2Param, 
                        initialGuess[0].Value, 
                        initialGuess[1].Value, 
                        FitTolerance, 
                        FitMaxIteration);
                    rawFitParam = TupleToList(fitParam);
                    break;
                case Func<double, double, double, double, double> func3Param:
                    var fitParamB = Fit.Curve(x, y, 
                        func3Param, 
                        initialGuess[0].Value, 
                        initialGuess[1].Value, 
                        initialGuess[2].Value, 
                        FitTolerance, 
                        FitMaxIteration);
                    rawFitParam = TupleToList(fitParamB);
                    break;
                case Func<double, double, double, double, double, double> func4Param:
                    var fitParamC = Fit.Curve(x, y,
                        func4Param, 
                        initialGuess[0].Value, 
                        initialGuess[1].Value,
                        initialGuess[2].Value,
                        initialGuess[3].Value,
                        FitTolerance, 
                        FitMaxIteration);
                    rawFitParam = TupleToList(fitParamC);
                    break;
                case Func<double, double, double, double, double, double, double> func5Param:
                    var fitParamD = Fit.Curve(x, y,
                        func5Param,
                        initialGuess[0].Value,
                        initialGuess[1].Value,
                        initialGuess[2].Value,
                        initialGuess[3].Value,
                        initialGuess[4].Value,
                        FitTolerance,
                        FitMaxIteration);
                    rawFitParam = TupleToList(fitParamD);
                    break;
            }
            for (int i = 0; i < rawFitParam.Count; i++)
            {
                Parameters[i].Value = (double)rawFitParam[i];
            }

            double[] yFit = GetY(x);
            CoeffOfDetermination = GoodnessOfFit.CoefficientOfDetermination(y, yFit);
            OnFitPerformed(this, x, yFit);
            return CoeffOfDetermination;
        }

        private static List<object> TupleToList(ITuple tuple)
        {
            return Enumerable.Range(0, tuple.Length)
                             .Select(i => tuple[i])
                             .ToList();
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
            {
                List<double> args = Parameters.Select(p => p.Value).ToList();
                args.Add(xData[i]);
                yData[i] = InvokeFunc(function, args);
            }

            if (Randomness) AddRandomness(ref yData);

            return yData;
        }

        private static double InvokeFunc(object funcObj, List<double> args)
        {
            // Check if funcObj is a Func delegate
            if (funcObj is not Delegate func)
            {
                throw new ArgumentException("Argument is not a Func delegate.");
            }

            // Get the number of parameters expected by the Func delegate
            int numParameters = func.Method.GetParameters().Length;

            // Check if the number of arguments matches the expected number of parameters
            if (args.Count != numParameters)
            {
                throw new ArgumentException($"Number of arguments ({args.Count}) does not match the expected number of parameters ({numParameters}) for the provided Func.");
            }

            // Convert the list of arguments to an array of object
            object[] parameters = args.Cast<object>().ToArray();

            // Invoke the Func delegate with the arguments
            object result = func.DynamicInvoke(parameters);

            // Convert the result to double (assuming the result is of type double)
            return Convert.ToDouble(result);
        }
    }
}
