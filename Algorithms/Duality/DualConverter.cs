using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.Algorithms.Duality
{
    public class DualConverter
    {
        public LPModel CreateDual(
            LPModel primal)
        {
            if (primal == null)
            {
                throw new ArgumentNullException(
                    nameof(primal));
            }

            if (!primal.IsMaximization())
            {
                throw new NotSupportedException(
                    "Dual conversion currently supports " +
                    "maximization primal models.");
            }

            foreach (Variable variable
                     in primal.Variables)
            {
                if (variable.Restriction != "+")
                {
                    throw new NotSupportedException(
                        "Dual conversion currently supports " +
                        "non-negative primal variables only.");
                }
            }

            LPModel dual =
                new LPModel
                {
                    ObjectiveType = "min"
                };


            for (int i = 0;
                 i < primal.Constraints.Count;
                 i++)
            {
                Constraint primalConstraint =
                    primal.Constraints[i];

                string restriction;

                if (primalConstraint.Relation == "<=")
                {
                    restriction = "+";
                }
                else if (primalConstraint.Relation == ">=")
                {
                    restriction = "-";
                }
                else if (primalConstraint.Relation == "=")
                {
                    restriction = "urs";
                }
                else
                {
                    throw new NotSupportedException(
                        $"Unsupported constraint relation: " +
                        $"{primalConstraint.Relation}");
                }

                double objectiveCoefficient =
                    primalConstraint.RightHandSide;

                char sign =
                    objectiveCoefficient < 0
                        ? '-'
                        : '+';

                Variable dualVariable =
                    new Variable(
                        $"y{i + 1}",
                        Math.Abs(
                            objectiveCoefficient),
                        sign,
                        restriction);

                dual.Variables.Add(
                    dualVariable);
            }


            for (int j = 0;
                 j < primal.Variables.Count;
                 j++)
            {
                List<double> coefficients =
                    new();

                List<char> signs =
                    new();

                for (int i = 0;
                     i < primal.Constraints.Count;
                     i++)
                {
                    double coefficient =
                        primal.Constraints[i]
                            .GetSignedCoefficients()[j];

                    coefficients.Add(
                        Math.Abs(coefficient));

                    signs.Add(
                        coefficient < 0
                            ? '-'
                            : '+');
                }

                double rhs =
                    primal.Variables[j]
                        .GetSignedObjectiveCoefficient();

                Constraint dualConstraint =
                    new Constraint(
                        coefficients,
                        signs,
                        ">=",
                        rhs);

                dual.Constraints.Add(
                    dualConstraint);
            }

            return dual;
        }
    }
}
