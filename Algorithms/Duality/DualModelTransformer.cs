using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.Algorithms.Duality
{
    public class DualModelTransformer
    {
        public DualTransformationResult TransformForSimplex(
            LPModel dual)
        {
            LPModel transformed =
                new LPModel
                {
                    ObjectiveType = "max"
                };

            Dictionary<string, string> mappings =
                new();

            /*
             We transform:
             
             y >= 0
                 stays y >= 0
             
              y <= 0
                  becomes y = -yNeg,
                  where yNeg >= 0
             
              y unrestricted
                  becomes:
                  y = yPos - yNeg
             */
            foreach (Variable variable
                     in dual.Variables)
            {
                double c =
                    variable.GetSignedObjectiveCoefficient();

                if (variable.Restriction == "+")
                {
                    /*
                      Dual is MIN.
                     
                      We solve the equivalent:
                     
                      MAX -W
                     
                      So objective coefficient is -c.
                     */
                    AddVariable(
                        transformed,
                        variable.Name,
                        -c);

                    mappings[variable.Name] =
                        variable.Name;
                }
                else if (variable.Restriction == "-")
                {
                    string newName =
                        variable.Name + "Neg";

                    /*
                      y = -yNeg
                     
                      Original dual objective:
                      c*y
                     
                      becomes:
                      -c*yNeg
                     
                      Then converting MIN to MAX
                      changes sign again:
                     
                      +c*yNeg
                     */
                    AddVariable(
                        transformed,
                        newName,
                        c);

                    mappings[variable.Name] =
                        $"-{newName}";
                }
                else if (variable.Restriction == "urs")
                {
                    string positiveName =
                        variable.Name + "Pos";

                    string negativeName =
                        variable.Name + "Neg";

                    /*
                      y = yPos - yNeg
                     */
                    AddVariable(
                        transformed,
                        positiveName,
                        -c);

                    AddVariable(
                        transformed,
                        negativeName,
                        c);

                    mappings[variable.Name] =
                        $"{positiveName} - {negativeName}";
                }
                else
                {
                    throw new NotSupportedException(
                        $"Unsupported dual variable restriction: " +
                        $"{variable.Restriction}");
                }
            }

            /*
              Transform every dual constraint.
             */
            foreach (Constraint constraint
                     in dual.Constraints)
            {
                List<double> original =
                    constraint.GetSignedCoefficients();

                List<double> transformedCoefficients =
                    new();

                for (int i = 0;
                     i < dual.Variables.Count;
                     i++)
                {
                    Variable variable =
                        dual.Variables[i];

                    double coefficient =
                        original[i];

                    if (variable.Restriction == "+")
                    {
                        transformedCoefficients.Add(
                            coefficient);
                    }
                    else if (variable.Restriction == "-")
                    {
                        transformedCoefficients.Add(
                            -coefficient);
                    }
                    else if (variable.Restriction == "urs")
                    {
                        transformedCoefficients.Add(
                            coefficient);

                        transformedCoefficients.Add(
                            -coefficient);
                    }
                }

                /*
                  Because we're converting the
                  minimization dual into a maximization
                  problem, we can multiply the dual
                  >= constraints by -1:
                 
                  A y >= c
                 
                  becomes:
                 
                  -A y <= -c
                 */
                List<double> finalCoefficients =
                    new();

                List<char> signs =
                    new();

                foreach (double coefficient
                         in transformedCoefficients)
                {
                    double negated =
                        -coefficient;

                    finalCoefficients.Add(
                        Math.Abs(negated));

                    signs.Add(
                        negated < 0
                            ? '-'
                            : '+');
                }

                double rhs =
                    -constraint.RightHandSide;

                /*
                 * Your CanonicalConverter currently rejects
                 * negative RHS values, so we'll deal with
                 * this problem separately below.
                 */
                Constraint transformedConstraint =
                    new Constraint(
                        finalCoefficients,
                        signs,
                        "<=",
                        rhs);

                transformed.Constraints.Add(
                    transformedConstraint);
            }

            return new DualTransformationResult
            {
                TransformedModel =
                    transformed,

                VariableMappings =
                    mappings
            };
        }

        private void AddVariable(
            LPModel model,
            string name,
            double coefficient)
        {
            char sign =
                coefficient < 0
                    ? '-'
                    : '+';

            Variable variable =
                new Variable(
                    name,
                    Math.Abs(coefficient),
                    sign,
                    "+");

            model.Variables.Add(
                variable);
        }
    }
}
