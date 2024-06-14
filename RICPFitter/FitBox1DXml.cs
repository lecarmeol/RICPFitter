using RICPFitter.Collections;
using RICPFitter.Data;
using RICPFitter.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter
{
    /// <summary>
    /// Class designed to fit any 1D curve on input data
    /// </summary>
    public class FitBox1DXml : IFitBox1D
    {
        private IFittable fitFunction;
        private IDataSet inputData;

        /// <inheritdoc/>
        public IDataSet InputData
        {
            get => inputData;
            set
            {
                if (inputData != value)
                {
                    if (inputData != null) inputData.InputDataChanged -= InputData_InputDataChanged;
                    inputData = value;
                    if (inputData != null)
                    {
                        InputData_InputDataChanged(inputData);
                        inputData.InputDataChanged += InputData_InputDataChanged;
                    }
                }
            }
        }

        private void InputData_InputDataChanged(IDataSet data)
        {
            InputDataChanged?.Invoke(data.XData, data.YData);
        }

        /// <inheritdoc/>
        public IFittable FitFunction
        {
            get => fitFunction;
            set
            {
                if (fitFunction != value)
                {
                    if (fitFunction != null) fitFunction.FitPerformed -= FitFunction_FitPerformed;
                    fitFunction = value;
                    if (fitFunction != null)
                    {
                        FitFunctionChanged?.Invoke(fitFunction);
                        fitFunction.FitPerformed += FitFunction_FitPerformed;
                    }
                }
            }
        }

        private void FitFunction_FitPerformed(IFittable func, double[] x, double[] y)
        {
            FitDataChanged?.Invoke(func, x, y);
        }

        /// <inheritdoc/>
        public IFunctionCollection ListOfFitFunctions { get; set; }

        /// <summary>
        /// New empty fit box 1D
        /// </summary>
        public FitBox1DXml()
        {

        }

        /// <summary>
        /// New fit box 1D with fit functions definitions
        /// </summary>
        /// <param name="xmlFilePath">XML file path containing the definitions of the fit functions</param>
        public FitBox1DXml(string xmlFilePath)
        {
            ListOfFitFunctions = new FunctionCollectionXml(xmlFilePath);
            if (ListOfFitFunctions != null)
            {
                FitFunction = ListOfFitFunctions.Functions[0];
            }
        }

        /// <inheritdoc/>
        public event Action<double[], double[]> InputDataChanged;
        /// <inheritdoc/>
        public event Action<IFittable, double[], double[]> FitDataChanged;
        /// <inheritdoc/>
        public event Action<IFittable> FitFunctionChanged;
    }
}
