using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Models
{
    public class Constraint 
    {
        public List<double> Coefficients { get; set; }
        public List<char> Signs { get; set; }
        public string Relation { get; set; }
        public double RightHandSide { get; set; }
        public Constraint(
            List<double> coefficients,
            List<char> signs,
            string relation,
            double rightHandSide)
        {
            Coefficients = coefficients;
            Signs = signs;
            Relation = relation;
            RightHandSide = rightHandSide;
        }
        public List<double> GetSignedCoefficients()
        {
            List<double> result = new();
            for (int i = 0; i < Coefficients.Count; i++)
            {
                double value = Coefficients[i];
                if (Signs[i] == '-')
                {
                    value = -value;
                }
                result.Add(value);
            }
            return result;
        }
    }
}
