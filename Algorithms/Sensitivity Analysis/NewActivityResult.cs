using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Algorithms.Sensitivity_Analysis
{
    public class NewActivityResult
    {
        public string VariableName { get; set; } = "";

        public double ObjectiveCoefficient { get; set; }

        public List<double> ConstraintCoefficients{get;set;} = new();

        public double ReducedCost { get; set; }

        public bool CanImproveSolution { get; set; }

        public string GetFormattedResult()
        {
            string result =
                $"New Activity: {VariableName}\r\n" +
                $"Objective Coefficient: " +
                $"{ObjectiveCoefficient:0.###}\r\n" +
                $"Reduced Cost (Zj - Cj): " +
                $"{ReducedCost:0.###}\r\n";

            if (CanImproveSolution)
            {
                result +="Result: The new activity can improve the current optimal solution.\r\n";
            }
            else
            {
                result +="Result: The new activity does not improve the current optimal solution.\r\n";
            }

            return result;
        }
    }
}
