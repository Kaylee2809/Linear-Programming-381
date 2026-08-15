using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Core
{
    public class CanonicalConverter
    {
        public Tableau Convert(
            LPModel model)
        {
            if (!model.IsMaximization())
            {
                throw new NotSupportedException(
                    "Primal Simplex currently requires " +
                    "a maximization problem.");
            }
            foreach (Variable variable
                     in model.Variables)
            {
                if (variable.Restriction != "+" &&
                    variable.Restriction != "int" &&
                    variable.Restriction != "bin")
                {
                    throw new NotSupportedException(
                        "Primal Simplex requires " +
                        "non-negative variables.");
                }
            }
            foreach (Constraint constraint
                     in model.Constraints)
            {
                if (constraint.Relation != "<=")
                {
                    throw new NotSupportedException(
                        "Primal Simplex currently requires " +
                        "<= constraints.");
                }
                if (constraint.RightHandSide < 0)
                {
                    throw new NotSupportedException(
                        "Negative RHS values require " +
                        "additional preprocessing.");
                }
            }
            int variableCount =
                model.Variables.Count;
            int constraintCount =
                model.Constraints.Count;
            int totalColumns =
                variableCount +
                constraintCount +
                1;
            int totalRows =
                constraintCount + 1;
            double[,] values =
                new double[
                    totalRows,
                    totalColumns];
            List<string> columnNames =
                new();
            List<string> basicVariables =
                new();
            // Decision variable columns.
            foreach (Variable variable
                     in model.Variables)
            {
                columnNames.Add(
                    variable.Name);
            }
            // Slack variable columns.
            for (int i = 0;
                 i < constraintCount;
                 i++)
            {
                columnNames.Add(
                    $"s{i + 1}");
            }
            // RHS.
            columnNames.Add("RHS");
            // Constraint rows.
            for (int i = 0;
                 i < constraintCount;
                 i++)
            {
                Constraint constraint =
                    model.Constraints[i];
                List<double> coefficients =
                    constraint.GetSignedCoefficients();
                for (int j = 0;
                     j < variableCount;
                     j++)
                {
                    values[i, j] =
                        coefficients[j];
                }
                // Slack variable.
                values[
                    i,
                    variableCount + i] = 1;
                // RHS.
                values[
                    i,
                    totalColumns - 1] =
                    constraint.RightHandSide;
                basicVariables.Add(
                    $"s{i + 1}");
            }
            // Objective row.
            int objectiveRow =
                totalRows - 1;

            for (int j = 0;
                 j < variableCount;
                 j++)
            {
                values[
                    objectiveRow,
                    j] =
                    -model.Variables[j]
                        .GetSignedObjectiveCoefficient();
            }
            basicVariables.Add("Z");
            return new Tableau(
                values,
                columnNames,
                basicVariables);
        }
    }
}
