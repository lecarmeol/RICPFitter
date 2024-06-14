using RICPFitter.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RICPFitter.Collections
{
    /// <summary>
    /// Implementation of IFunctionCollection with XML interaction
    /// </summary>
    public class FunctionCollectionXml : IFunctionCollection
    {
        /// <inheritdoc/>
        public List<IFittable> Functions { get; set; } = [];

        /// <summary>
        /// New empty collection
        /// </summary>
        public FunctionCollectionXml()
        {
            
        }

        /// <summary>
        /// New collection from a list of functions
        /// </summary>
        /// <param name="listOfFunctions"></param>
        public FunctionCollectionXml(List<IFittable> listOfFunctions) : this()
        {
            Functions = listOfFunctions;
        }

        /// <summary>
        /// New collection from an xml file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public FunctionCollectionXml(string xmlFilePath) : this()
        {
            LoadFromXml(xmlFilePath);
        }

        /// <inheritdoc/>
        public IFittable FindByName(string name, string category = null)
        {
            if (category != null)
            {
                return Functions.Find(func => func.Name == name &&  func.Category == category);
            }
            else
            {
                return Functions.Find(func => func.Name == name);
            }
        }

        /// <summary>
        /// Load a collection from an xml file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <exception cref="Exception"></exception>
        public void LoadFromXml(string xmlFilePath)
        {
            XmlDocument xmlDoc = new();
            xmlDoc.Load(xmlFilePath);
            if (xmlDoc.DocumentElement == null) throw new Exception($"File {xmlFilePath} is empty");
            if (xmlDoc.DocumentElement.Name != "Functions") throw new Exception($"File {xmlFilePath} is not a relevant configuration file");
            foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
            {
                if (node.Name == "Function")
                {
                    Functions.Add(ExternalFunc.FromXml(node));
                }
            }
        }

        /// <summary>
        /// Write a collection into an xml file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public void ToXml(string xmlFilePath)
        {
            XmlDocument xmlDocument = new();

            XmlElement parentNode = xmlDocument.CreateElement("Functions");
            foreach(IFittable func in Functions)
            {
                if (func is ExternalFunc extFunc)
                {
                    extFunc.ToXml(parentNode);
                }
            }
            xmlDocument.AppendChild(parentNode);
            xmlDocument.Save(xmlFilePath);
        }

        /// <inheritdoc/>
        public void SortByCategoryAndName()
        {
            Functions.Sort(new FunctionComparer());
        }

        /// <inheritdoc/>
        public object Clone()
        {
            IFunctionCollection result = new FunctionCollectionXml(new List<IFittable>(Functions));
            return result;
        }
    }

    /// <summary>
    /// Comparer to sort Function by Category and Name
    /// </summary>
    public class FunctionComparer : IComparer<IFittable>
    {
        /// <inheritdoc/>
        public int Compare(IFittable x, IFittable y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentException("Cannot compare null functions");
            }

            int categoryComparison = string.Compare(x.Category, y.Category, StringComparison.Ordinal);
            if (categoryComparison != 0)
            {
                return categoryComparison;
            }

            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }
    }
}
