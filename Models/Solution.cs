using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Models
{
    public class Solution
    {
        public bool IsFeasible { get; set; }
        public bool IsOptimal { get; set; }
        public bool IsUnbounded { get; set; }
        public bool IsInfeasible { get; set; }
        public double ObjectiveValue { get; set; }
        public Dictionary<string, double> VariableValues { get; set; }
        public List<string> Iterations { get; set; } 
        public Solution()
        { 
            IsFeasible = true;
            IsOptimal = false;
            IsUnbounded = false;
            IsInfeasible = false;
            VariableValues = new Dictionary<string, double>();
            Iterations = new List<string>();
        }
    }
}
