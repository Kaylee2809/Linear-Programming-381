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

            if(tableau.ArtificialVariables.Count > 0)
            {
                solution.Iterations.Add(
                    "========== PHASE 1 ==========\n" +
                    "Artificial variables detected.\n" +
                    "Finding a feasible starting solution.\n"
                );

                PreparePhaseOneObjective(tableau);

                bool phaseOneSuccessful = RunSimplex(tableau, solution, ref iteration, new HashSet<string>());

                if (!phaseOneSuccessful)
                {
                    solution.IsFeasible = false;
                    solution.IsOptimal = false;
                    solution.IsInfeasible = true;

                    solution.Iterations.Add("Phase 1 FAILED.");
                    return solution;
                }

                int objectiveRow = tableau.RowCount - 1;

                int rhsColumn = tableau.ColumnCount - 1;

                double phaseOneObjective = tableau.Values[objectiveRow, rhsColumn];

                //Phase 1 maximised, so best possible value is 0. If its still a -, then an artificial var remains positive

                if(phaseOneObjective < -MathUtilities.Epsilon)
                {
                    solution.IsFeasible = false;
                    solution.IsOptimal = false;
                    solution.IsInfeasible = true;

                    solution.Iterations.Add("PROBLEM IS INFEASIBLE.\n" +
                    "Phase 1 could not reduce all artificial variables to 0.");

                    return solution;
                }

                solution.Iterations.Add("PHASE 1 COMPLETE. \n" +
                "A feasible solution was found.\n");

                RemoveArtificialVariablesFromBasis(tableau);

                solution.Iterations.Add("========== PHASE 2 ==========\n" +
                "Restoring the original objective function.\n");

                PreparePhaseTwoObjective(model, tableau);

                iteration = 0;
            }

            HashSet<string> excludedColumns = new HashSet<string>(tableau.ArtificialVariables);

            bool successful = RunSimplex(tableau, solution, ref iteration, excludedColumns);

            if (!successful)
            {
                return solution;
            }

            solution.IsOptimal = true;
            solution.IsFeasible = true;

            ExtractSolution(model, tableau, solution);

            return solution;
        }

        private bool RunSimplex(Tableau tableau, Solution solution, ref int iteration, HashSet<string> excludedColumns)
        {
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
                        objectiveRow,
                        excludedColumns);
                // No negative coefficients in the objective row means the solution is optimal.
                if (enteringColumn == -1)
                {
                    return true;
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
                        return false;
                   // return solution;
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
            int objectiveRow,
            HashSet<string> excludedColumns)
        {
            int enteringColumn = -1;
            double mostNegative = 0;

            // Do not include RHS.
            for (int j = 0;
                 j < tableau.ColumnCount - 1;
                 j++)
            {
                string columnName = tableau.ColumnNames[j];

                //During phase 2, artificial variables are not allowed to enter the basis

                if (excludedColumns.Contains(columnName))
                {
                    continue;
                }


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

        private void PreparePhaseOneObjective(Tableau tableau)
        {
            int objectiveRow = tableau.RowCount - 1;

            int rhsColumn = tableau.ColumnCount -1;

            for(int j = 0; j < tableau.ColumnCount; j++)
            {
                tableau.Values[objectiveRow, j] = 0;
            }

            foreach(string artificialVariable in tableau.ArtificialVariables)
            {
                int column = tableau.ColumnNames.IndexOf(artificialVariable);

                if(column != -1)
                {
                    tableau.Values[objectiveRow, column] = 1;
                }
            }

            for(int i = 0; i < objectiveRow; i++)
            {
                string basicVariable = tableau.BasicVariables[i];

                if (!tableau.ArtificialVariables.Contains(basicVariable))
                {
                    continue;
                }

                int artificialColumn = tableau.ColumnNames.IndexOf(basicVariable);

                if(artificialColumn == -1)
                {
                    continue;
                }

                double factor = tableau.Values[objectiveRow, artificialColumn];

                if (MathUtilities.IsZero(factor))
                {
                    continue;
                }

                for(int j = 0; j < tableau.ColumnCount; j++)
                {
                    tableau.Values[objectiveRow, j] -= factor*tableau.Values[i,j];
                }
            }
        }
        private void RemoveArtificialVariablesFromBasis(Tableau tableau)
        {
            int objectiveRow = tableau.RowCount - 1;

            for(int i = 0; i < objectiveRow; i++)
            {
                string basicVariable = tableau.BasicVariables[i];

                if (!tableau.ArtificialVariables.Contains(basicVariable))
                {
                    continue;
                }

                int pivotColumn = -1;

                for(int j = 0; j < tableau.ColumnCount - 1; j++)
                {
                    string columnName = tableau.ColumnNames[j];

                    if (tableau.ArtificialVariables.Contains(columnName))
                    {
                        continue;
                    }

                    if (!MathUtilities.IsZero(tableau.Values[i, j]))
                    {
                        pivotColumn = j;
                        break;
                    }
                }
                if(pivotColumn != -1)
                {
                    tableau.Pivot(i, pivotColumn);
                }
            }
        }

        private void PreparePhaseTwoObjective(LPModel model, Tableau tableau)
        {
            int objectiveRow = tableau.RowCount - 1;

            //Clear Phase 1 objective row.

            for(int i = 0; i < tableau.ColumnCount; i++)
            {
                tableau.Values[objectiveRow, i] = 0;
            }

            //Restore original objective coefficients
            foreach(Variable variable in model.Variables)
            {
                int column = tableau.ColumnNames.IndexOf(variable.Name);

                if(column == -1)
                {
                    continue;
                }

                tableau.Values[objectiveRow, column] = -variable.GetSignedObjectiveCoefficient();
            }

            for(int i = 0; i < objectiveRow; i++)
            {
                string basicVariable = tableau.BasicVariables[i];

                int basicColumn = tableau.ColumnNames.IndexOf(basicVariable);

                if(basicColumn == -1)
                {
                    continue;
                }

                double factor = tableau.Values[objectiveRow, basicColumn];

                if (MathUtilities.IsZero(factor))
                {
                    continue;
                }

                for(int j = 0; j < tableau.ColumnCount; j++)
                {
                    tableau.Values[objectiveRow, j] -= factor * tableau.Values[i,j];
                }
            }
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