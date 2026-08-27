using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.Algorithms.Duality
{
    public class DualTransformationResult
    {
        public LPModel TransformedModel
        {
            get;
            set;
        } = null!;

        public Dictionary<string, string> VariableMappings
        {
            get;
            set;
        } = new();
    }
}
