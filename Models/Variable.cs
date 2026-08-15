using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Models
{
    public class Variable 
    {
        public string Name { get; set; }
        public double ObjectiveCoefficient { get; set; }
        public char ObjectiveSign { get; set; }
        public string Restriction { get; set; }
        public Variable(string name, double objectiveCoefficient, char objectiveSign, string restriction)
        {
            Name = name;
            ObjectiveCoefficient = objectiveCoefficient;
            ObjectiveSign = objectiveSign;
            Restriction = restriction;
        }
        public double GetSignedObjectiveCoefficient()
        {
            return ObjectiveSign == '-'
                ? -ObjectiveCoefficient
                : ObjectiveCoefficient;
        }
        public bool IsBinary()
        {
            return Restriction.Equals(
                "bin",
                StringComparison.OrdinalIgnoreCase);
        }
        public bool IsInteger()
        {
            return Restriction.Equals(
                "int",
                StringComparison.OrdinalIgnoreCase);
        }
        public bool IsUnrestricted()
        {
            return Restriction.Equals(
                "urs",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}