using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Models
{
    public class LPModel
    {
        public string ObjectiveType { get; set; }
        public List<Variable> Variables { get; set; }
        public List<Constraint> Constraints { get; set; }
        public LPModel()
        {
            ObjectiveType = "max";
            Variables = new List<Variable>();
            Constraints = new List<Constraint>();
        }
        public bool IsMaximization()
        {
            return ObjectiveType.Equals(
                "max",
                StringComparison.OrdinalIgnoreCase);
        }
        public bool IsMinimization()
        {
            return ObjectiveType.Equals(
                "min",
                StringComparison.OrdinalIgnoreCase);
        }
        public string GetObjectiveString()
        {
            string result =
                $"{ObjectiveType.ToUpper()} Z = ";

            for (int i = 0; i < Variables.Count; i++)
            {
                double coefficient =
                    Variables[i]
                        .GetSignedObjectiveCoefficient();
                if (i > 0)
                {
                    result += coefficient >= 0
                        ? " + "
                        : " - ";
                }
                else if (coefficient < 0)
                {
                    result += "-";
                }
                result +=
                    $"{Math.Abs(coefficient):0.###}" +
                    Variables[i].Name;
            }
            return result;
        }
        public string GetConstraintString(
            Constraint constraint)
        {
            List<double> coefficients =
                constraint.GetSignedCoefficients();
            string result = "";
            for (int i = 0; i < coefficients.Count; i++)
            {
                double coefficient =
                    coefficients[i];

                if (i > 0)
                {
                    result += coefficient >= 0
                        ? " + "
                        : " - ";
                }
                else if (coefficient < 0)
                {
                    result += "-";
                }
                result +=
                    $"{Math.Abs(coefficient):0.###}" +
                    Variables[i].Name;
            }
            result +=
                $" {constraint.Relation} " +
                $"{constraint.RightHandSide:0.###}";
            return result;
        }
        public string GetRestrictionsString()
        {
            return string.Join(
                " ",
                Variables.Select(v => v.Restriction));
        }
    }
}