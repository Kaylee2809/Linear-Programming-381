using Linear_Programming_381.Core;
using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Algorithms.Primal_Simplex
{
    public class PrimalSimplexSolver
    {
        public Solution Solve(
            LPModel model,
            Tableau tableau)
        {
            Solution solution = new();
            int iteration = 0;
            while (true)
            {
                string currentTableau =
                    CreateIterationOutput(
                        tableau,
                        iteration);
                solution.Iterations.Add(
                    currentTableau);
                int objectiveRow =
                    tableau.RowCount - 1;
                int enteringColumn =
                    FindEnteringVariable(
                        tableau,
                        objectiveRow);
                // No negative coefficients in the objective row means the solution is optimal.
                if (enteringColumn == -1)
                {
                    solution.IsOptimal = true;
                    solution.IsFeasible = true;
                    ExtractSolution(
                        model,
                        tableau,
                        solution);
                    return solution;
                }
                int leavingRow =
                    FindLeavingVariable(
                        tableau,
                        enteringColumn);
                // No positive coefficient means the objective can increase indefinitely.
                if (leavingRow == -1)
                {
                    solution.IsUnbounded = true;
                    solution.IsFeasible = false;
                    solution.Iterations.Add(
                        "PROBLEM IS UNBOUNDED.");
                    return solution;
                }
                tableau.Pivot(
                    leavingRow,
                    enteringColumn);
                iteration++;
            }
        }
        private string CreateIterationOutput(
            Tableau tableau,
            int iteration)
        {
            StringBuilder output =
                new();
            output.AppendLine(
                $"========== ITERATION {iteration} ==========");
            output.AppendLine();
            output.Append(
                tableau.ToFormattedString());
            output.AppendLine();
            return output.ToString();
        }
        private int FindEnteringVariable(
            Tableau tableau,
            int objectiveRow)
        {
            int enteringColumn = -1;
            double mostNegative = 0;
            // Do not include RHS.
            for (int j = 0;
                 j < tableau.ColumnCount - 1;
                 j++)
            {
                double value =
                    tableau.Values[
                        objectiveRow,
                        j];
                if (value < mostNegative &&
                    !MathUtilities.IsZero(value))
                {
                    mostNegative = value;

                    enteringColumn = j;
                }
            }
            return enteringColumn;
        }
        private int FindLeavingVariable(
            Tableau tableau,
            int enteringColumn)
        {
            int leavingRow = -1;
            double smallestRatio =
                double.PositiveInfinity;
            int objectiveRow =
                tableau.RowCount - 1;
            for (int i = 0;
                 i < objectiveRow;
                 i++)
            {
                double coefficient =
                    tableau.Values[
                        i,
                        enteringColumn];
                double rhs =
                    tableau.Values[
                        i,
                        tableau.ColumnCount - 1];
                if (coefficient >
                    MathUtilities.Epsilon)
                {
                    double ratio =
                        rhs / coefficient;
                    if (ratio >= 0 &&
                        ratio < smallestRatio)
                    {
                        smallestRatio = ratio;
                        leavingRow = i;
                    }
                }
            }
            return leavingRow;
        }
        private void ExtractSolution(
            LPModel model,
            Tableau tableau,
            Solution solution)
        {
            int rhsColumn =
                tableau.ColumnCount - 1;
            int objectiveRow =
                tableau.RowCount - 1;
            solution.ObjectiveValue =
                tableau.Values[
                    objectiveRow,
                    rhsColumn];
            foreach (Variable variable
                     in model.Variables)
            {
                int column =
                    tableau.ColumnNames
                        .IndexOf(variable.Name);
                double value = 0;
                if (column != -1)
                {
                    for (int i = 0;
                         i < objectiveRow;
                         i++)
                    {
                        if (tableau.BasicVariables[i]
                            == variable.Name)
                        {
                            value =
                                tableau.Values[
                                    i,
                                    rhsColumn];
                            break;
                        }
                    }
                }
                solution.VariableValues[
                    variable.Name] = value;
            }
        }
    }
}