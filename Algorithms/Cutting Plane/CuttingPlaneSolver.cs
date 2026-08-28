using Linear_Programming_381.Core;
using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linear_Programming_381.Algorithms.Cutting_Plane
{
    // Solves an integer LP with Gomory's Cutting Plane method: solve the
    // LP relaxation with ordinary Primal Simplex, then repeatedly add a
    // fractional (Gomory) cut and re-optimize with Dual Simplex until
    // every integer-restricted variable is integer-valued.
    public class CuttingPlaneSolver
    {
        private const int MaxCuts = 50; // safety limit against cycling.

        public Solution Solve(LPModel model)
        {
            Solution solution = new();
            CanonicalConverter converter = new();
            Tableau tableau = converter.Convert(model);

            if (!SolveToOptimal(tableau, solution, "RELAXATION"))
            {
                return solution; // unbounded, already logged.
            }

            List<Variable> integerVariables = model.Variables
                .Where(v => v.IsInteger() || v.IsBinary())
                .ToList();

            if (integerVariables.Any(v => v.IsBinary()))
            {
                solution.Iterations.Add(
                    "NOTE: binary upper-bound constraints (x <= 1) are not " +
                    "enforced by this Cutting Plane implementation -- pair " +
                    "with Branch & Bound Knapsack for the binary Knapsack model.");
            }

            int cutNumber = 0;

            while (true)
            {
                int fractionalRow = FindFractionalRow(tableau, integerVariables);

                if (fractionalRow == -1)
                {
                    solution.IsOptimal = true;
                    solution.IsFeasible = true;
                    ExtractSolution(model, tableau, solution);
                    return solution;
                }

                if (cutNumber >= MaxCuts)
                {
                    solution.IsInfeasible = true;
                    solution.IsFeasible = false;
                    solution.Iterations.Add(
                        "CUTTING PLANE DID NOT CONVERGE WITHIN THE ITERATION LIMIT.");
                    return solution;
                }

                cutNumber++;
                tableau = AddGomoryCut(tableau, fractionalRow, cutNumber, solution);

                if (!SolveDualToOptimal(tableau, solution, cutNumber))
                {
                    solution.IsInfeasible = true;
                    solution.IsFeasible = false;
                    solution.Iterations.Add(
                        $"PROBLEM IS INFEASIBLE (no valid pivot after cut {cutNumber}).");
                    return solution;
                }
            }
        }

        private bool SolveToOptimal(Tableau tableau, Solution solution, string tag)
        {
            int iteration = 0;
            int objectiveRow = tableau.RowCount - 1;

            while (true)
            {
                solution.Iterations.Add(CreateIterationOutput(tableau, tag, iteration));

                int enteringColumn = FindEnteringVariable(tableau, objectiveRow);

                if (enteringColumn == -1)
                {
                    return true;
                }

                int leavingRow = FindLeavingVariable(tableau, enteringColumn, objectiveRow);

                if (leavingRow == -1)
                {
                    solution.IsUnbounded = true;
                    solution.IsFeasible = false;
                    solution.Iterations.Add("PROBLEM IS UNBOUNDED.");
                    return false;
                }

                tableau.Pivot(leavingRow, enteringColumn);
                iteration++;
            }
        }

        private bool SolveDualToOptimal(Tableau tableau, Solution solution, int cutNumber)
        {
            int iteration = 0;
            int objectiveRow = tableau.RowCount - 1;

            while (true)
            {
                int leavingRow = FindMostInfeasibleRow(tableau, objectiveRow);

                if (leavingRow == -1)
                {
                    solution.Iterations.Add(
                        CreateIterationOutput(tableau, $"CUT {cutNumber}", iteration));
                    return true;
                }

                int enteringColumn = FindDualEnteringColumn(tableau, leavingRow, objectiveRow);

                solution.Iterations.Add(
                    CreateIterationOutput(tableau, $"CUT {cutNumber}", iteration));

                if (enteringColumn == -1)
                {
                    return false;
                }

                tableau.Pivot(leavingRow, enteringColumn);
                iteration++;
            }
        }

        private int FindEnteringVariable(Tableau tableau, int objectiveRow)
        {
            int enteringColumn = -1;
            double mostNegative = 0;

            for (int j = 0; j < tableau.ColumnCount - 1; j++)
            {
                double value = tableau.Values[objectiveRow, j];
                if (value < mostNegative && !MathUtilities.IsZero(value))
                {
                    mostNegative = value;
                    enteringColumn = j;
                }
            }
            return enteringColumn;
        }

        private int FindLeavingVariable(Tableau tableau, int enteringColumn, int objectiveRow)
        {
            int leavingRow = -1;
            double smallestRatio = double.PositiveInfinity;

            for (int i = 0; i < objectiveRow; i++)
            {
                double coefficient = tableau.Values[i, enteringColumn];
                double rhs = tableau.Values[i, tableau.ColumnCount - 1];

                if (coefficient > MathUtilities.Epsilon)
                {
                    double ratio = rhs / coefficient;
                    if (ratio >= 0 && ratio < smallestRatio)
                    {
                        smallestRatio = ratio;
                        leavingRow = i;
                    }
                }
            }
            return leavingRow;
        }

        private int FindMostInfeasibleRow(Tableau tableau, int objectiveRow)
        {
            int rhsColumn = tableau.ColumnCount - 1;
            int worstRow = -1;
            double mostNegative = -MathUtilities.Epsilon;

            for (int i = 0; i < objectiveRow; i++)
            {
                double rhs = tableau.Values[i, rhsColumn];
                if (rhs < mostNegative)
                {
                    mostNegative = rhs;
                    worstRow = i;
                }
            }
            return worstRow;
        }

        private int FindDualEnteringColumn(Tableau tableau, int leavingRow, int objectiveRow)
        {
            int enteringColumn = -1;
            double bestRatio = double.PositiveInfinity;

            for (int j = 0; j < tableau.ColumnCount - 1; j++)
            {
                double coefficient = tableau.Values[leavingRow, j];
                if (coefficient < -MathUtilities.Epsilon)
                {
                    double ratio = Math.Abs(tableau.Values[objectiveRow, j] / coefficient);
                    if (ratio < bestRatio)
                    {
                        bestRatio = ratio;
                        enteringColumn = j;
                    }
                }
            }
            return enteringColumn;
        }

        private int FindFractionalRow(Tableau tableau, List<Variable> integerVariables)
        {
            int objectiveRow = tableau.RowCount - 1;
            int rhsColumn = tableau.ColumnCount - 1;
            int bestRow = -1;
            double bestDistanceFromHalf = double.PositiveInfinity;

            for (int i = 0; i < objectiveRow; i++)
            {
                string basicVariableName = tableau.BasicVariables[i];
                bool isRestrictedInteger = integerVariables
                    .Any(v => v.Name == basicVariableName);

                if (!isRestrictedInteger)
                {
                    continue;
                }

                double rhsValue = tableau.Values[i, rhsColumn];
                if (MathUtilities.IsInteger(rhsValue))
                {
                    continue;
                }

                double distanceFromHalf = Math.Abs(FractionalPart(rhsValue) - 0.5);
                if (distanceFromHalf < bestDistanceFromHalf)
                {
                    bestDistanceFromHalf = distanceFromHalf;
                    bestRow = i;
                }
            }
            return bestRow;
        }

        private Tableau AddGomoryCut(
            Tableau tableau, int fractionalRow, int cutNumber, Solution solution)
        {
            int oldRows = tableau.RowCount;
            int oldColumns = tableau.ColumnCount;
            int newRows = oldRows + 1;
            int newColumns = oldColumns + 1;

            int rhsColumn = oldColumns - 1;
            int newCutColumn = newColumns - 2;
            int newRhsColumn = newColumns - 1;
            int cutRow = oldRows - 1;
            int newObjectiveRow = newRows - 1;
            int oldObjectiveRow = oldRows - 1;

            double[,] newValues = new double[newRows, newColumns];
            List<string> newColumnNames = new(tableau.ColumnNames);
            string cutVariableName = "c" + cutNumber;
            newColumnNames.Insert(newColumnNames.Count - 1, cutVariableName);

            List<string> newBasicVariables = new();

            for (int i = 0; i < oldRows - 1; i++)
            {
                for (int j = 0; j < rhsColumn; j++)
                {
                    newValues[i, j] = tableau.Values[i, j];
                }
                newValues[i, newRhsColumn] = tableau.Values[i, rhsColumn];
                newBasicVariables.Add(tableau.BasicVariables[i]);
            }

            double rhsFraction = FractionalPart(tableau.Values[fractionalRow, rhsColumn]);
            for (int j = 0; j < rhsColumn; j++)
            {
                newValues[cutRow, j] = -FractionalPart(tableau.Values[fractionalRow, j]);
            }
            newValues[cutRow, newCutColumn] = 1;
            newValues[cutRow, newRhsColumn] = -rhsFraction;
            newBasicVariables.Add(cutVariableName);

            for (int j = 0; j < rhsColumn; j++)
            {
                newValues[newObjectiveRow, j] = tableau.Values[oldObjectiveRow, j];
            }
            newValues[newObjectiveRow, newRhsColumn] = tableau.Values[oldObjectiveRow, rhsColumn];
            newBasicVariables.Add(tableau.BasicVariables[oldObjectiveRow]);

            Tableau newTableau = new(newValues, newColumnNames, newBasicVariables);

            solution.Iterations.Add(
                $"Gomory cut {cutNumber} derived from row '{tableau.BasicVariables[fractionalRow]}' " +
                $"(RHS fraction = {MathUtilities.Round(rhsFraction):0.###}): " +
                $"new constraint variable {cutVariableName} added.");

            return newTableau;
        }

        private double FractionalPart(double value)
        {
            double fraction = value - Math.Floor(value);
            return MathUtilities.IsZero(fraction) ? 0 : fraction;
        }

        private string CreateIterationOutput(Tableau tableau, string tag, int iteration)
        {
            StringBuilder output = new();
            output.AppendLine($"========== {tag} ITERATION {iteration} ==========");
            output.AppendLine();
            output.Append(tableau.ToFormattedString());
            output.AppendLine();

            // The objective row IS the Price-Out row: its non-basic entries
            // are each column's reduced cost after pricing out the current basis.
            int objectiveRow = tableau.RowCount - 1;
            output.AppendLine("Price-Out row (reduced costs, non-basic columns):");
            for (int j = 0; j < tableau.ColumnCount - 1; j++)
            {
                if (tableau.BasicVariables.Contains(tableau.ColumnNames[j]))
                {
                    continue;
                }
                output.AppendLine(
                    $"  {tableau.ColumnNames[j]}: c_bar = " +
                    $"{MathUtilities.Round(tableau.Values[objectiveRow, j]):0.###}");
            }
            output.AppendLine();

            return output.ToString();
        }

        private void ExtractSolution(LPModel model, Tableau tableau, Solution solution)
        {
            int rhsColumn = tableau.ColumnCount - 1;
            int objectiveRow = tableau.RowCount - 1;
            solution.ObjectiveValue = MathUtilities.Round(tableau.Values[objectiveRow, rhsColumn]);

            foreach (Variable variable in model.Variables)
            {
                double value = 0;
                for (int i = 0; i < objectiveRow; i++)
                {
                    if (tableau.BasicVariables[i] == variable.Name)
                    {
                        value = tableau.Values[i, rhsColumn];
                        break;
                    }
                }
                solution.VariableValues[variable.Name] = MathUtilities.Round(value);
            }
        }
    }
}