using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Algorithms.Sensitivity_Analysis
{
    public class NewConstraintResult
    {
        public List<double> Coefficients{get;set;} = new();

        public string Relation{get;set;} = "";

        public double RightHandSide{get;set;}

        public double LeftHandSideValue{get;set;}

        public bool CurrentSolutionSatisfiesConstraint{get;set;}

        public string GetFormattedResult()
        {
            string status =
                CurrentSolutionSatisfiesConstraint? "The current optimal solution satisfies the new constraint."
                    : "The current optimal solution violates the new constraint.";

            return
                $"Left-Hand-Side Value: {LeftHandSideValue:0.###}\r\n" +
                $"Relation: {Relation}\r\n" +
                $"Right-Hand-Side: {RightHandSide:0.###}\r\n" +
                $"Result: {status}\r\n";
        }
    }
}
