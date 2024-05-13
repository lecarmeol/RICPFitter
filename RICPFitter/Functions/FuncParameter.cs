using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RICPFitter.Functions
{
    /// <summary>
    /// Parameter of a function
    /// </summary>
    public class FuncParameter : IEquatable<FuncParameter>
    {
        /// <summary>
        /// Short name (usually a single letter)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Unit if relevant
        /// </summary>
        public string Unit { get; set; }

        public bool Equals(FuncParameter other)
        {
            return other.Name == Name && other.Description == Description;
        }

        public override bool Equals(object obj)
        {
            if (obj is FuncParameter parameter) return base.Equals(parameter);
            else return false;
            
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
