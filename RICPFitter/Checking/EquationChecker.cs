using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using RICPFitter.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RICPFitter.Checking
{
    /// <summary>
    /// Class containing a series of useful tool to test and validate equation
    /// </summary>
    public class EquationChecker
    {
        /// <summary>
        /// Check if a string is a valid equation
        /// </summary>
        /// <param name="equation">input equation as string</param>
        /// <param name="variableName">variable name</param>
        /// <param name="parametersName">parameters name</param>
        /// <param name="error">Error description if validation fails</param>
        /// <returns>result</returns>
        public static bool IsEquationValid(string equation, string variableName, List<FuncParameter> parameters, out string error)
        {
            // Wrap the equation in a small standalone program
            error = null;
            string wrappedEquation = @"
            using System;

            public class MyClass
            {
                public double MyMethod(";
            foreach (FuncParameter parameter in parameters)
            {
                wrappedEquation += "double " + parameter.Name + ",";
            }
            wrappedEquation += "double " + variableName + @")
                {
                    return " + equation + @";
                }
            }";

            return IsCSharpCodeValid(wrappedEquation, out error);
        }

        private static bool IsCSharpCodeValid(string code, out string error)
        {
            error = null;

            // Parse the code string into a syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

            // Create compilation options
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary);

            // Define the compilation
            var compilation = CSharpCompilation.Create("DynamicCodeValidation")
                                               .WithOptions(compilationOptions)
                                               .AddReferences(GetReferences());

            // Add the syntax tree to the compilation
            compilation = compilation.AddSyntaxTrees(syntaxTree);

            // Emit the compilation to memory
            using MemoryStream ms = new();
            EmitResult result = compilation.Emit(ms);

            // Check if there are any compilation errors
            if (!result.Success)
            {
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                // Output compilation errors (optional)
                StringBuilder sb = new();
                foreach (Diagnostic diagnostic in failures)
                {
                    sb.AppendLine($"{diagnostic.Id}: {diagnostic.GetMessage()} (Line {diagnostic.Location.GetLineSpan().StartLinePosition.Line})");
                }
                error = sb.ToString();

                return false;
            }
            else
            {
                return true;  // Code is valid
            }
        }


        // Helper method to get assembly references
        private static IEnumerable<MetadataReference> GetReferences()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                        .Select(a => MetadataReference.CreateFromFile(a.Location));
        }
    }

}