using Linear_Programming_381.Core;
using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linear_Programming_381.Algorithms.Revised_Simplex
{
    // Solves a maximization LP using the Revised Primal Simplex method:
    // instead of a full tableau, we maintain B-inverse (updated each
    // iteration via the Product Form of the Inverse) and "price out"
    // reduced costs from it each iteration. Handles <=, >=, and =
    // constraints via the Big-M method (surplus + artificial variables
    // for >=, artificial variables for =, penalized with a large cost).
    public class RevisedSimplexSolver
    {
        private const double BigM = 1_000_000;

        private int variableCount;
        private int constraintCount;
        private int totalColumns;
        private double[,] A = null!;
        private double[] c = null!;
        private double[] b = null!;
        private List<string> columnNames = null!;
        private Dictionary<int, (int row, double value)> specialColumns = null!;
        private List<int> artificialColumns = null!;

        // Builds the standard form (objective row + constraint rows with
        // their slack/surplus/artificial columns) WITHOUT solving --
        // used to display the Canonical Form before pivoting begins.
        public string GetCanonicalFormDisplay(LPModel model)
        {
            Validate(model);
            BuildStandardForm(model);

            const int width = 12;
            StringBuilder output = new();

            output.Append("Row".PadRight(width));
            foreach (string column in columnNames)
            {
                output.Append(column.PadLeft(width));
            }
            output.Append("RHS".PadLeft(width));
            output.AppendLine();

            for (int i = 0; i < constraintCount; i++)
            {
                output.Append(("R" + (i + 1)).PadRight(width));
                for (int j = 0; j < totalColumns; j++)
                {
                    output.Append(
                        GetColumn(j)[i].ToString("0.###").PadLeft(width));
                }
                output.Append(b[i].ToString("0.###").PadLeft(width));
                output.AppendLine();
            }

            output.Append("Z".PadRight(width));
            for (int j = 0; j < totalColumns; j++)
            {
                output.Append(c[j].ToString("0.###").PadLeft(width));
            }
            output.AppendLine();

            return output.ToString();
        }

        public Solution Solve(LPModel model)
        {
            Validate(model);
            BuildStandardForm(model);

            Solution solution = new();

            int[] basis = new int[constraintCount];
            for (int i = 0; i < constraintCount; i++)
            {
                basis[i] = FindInitialBasicColumn(i);
            }

            double[,] bInverse = Identity(constraintCount);
            int iteration = 0;

            while (true)
            {
                double[] xB = Multiply(bInverse, b);

                double[] cB = new double[constraintCount];
                for (int i = 0; i < constraintCount; i++)
                {
                    cB[i] = c[basis[i]];
                }

                // Price-out step: simplex multipliers y = c_B * B^-1.
                double[] y = MultiplyRowVector(cB, bInverse);

                double[] reducedCosts = new double[totalColumns];
                int enteringColumn = -1;
                double bestReducedCost = MathUtilities.Epsilon;

                for (int j = 0; j < totalColumns; j++)
                {
                    if (basis.Contains(j))
                    {
                        continue;
                    }

                    double[] column = GetColumn(j);
                    double reducedCost = c[j] - DotProduct(y, column);
                    reducedCosts[j] = reducedCost;

                    if (reducedCost > bestReducedCost)
                    {
                        bestReducedCost = reducedCost;
                        enteringColumn = j;
                    }
                }

                if (enteringColumn == -1)
                {
                    solution.Iterations.Add(
                        CreateIterationOutput(
                            iteration, basis, bInverse, xB, y,
                            reducedCosts, -1, -1, null));

                    int infeasibleRow = FindInfeasibleArtificialRow(basis, xB);
                    if (infeasibleRow != -1)
                    {
                        solution.IsInfeasible = true;
                        solution.IsFeasible = false;
                        solution.Iterations.Add(
                            "PROBLEM IS INFEASIBLE " +
                            "(an artificial variable remains basic and positive).");
                        return solution;
                    }

                    solution.IsOptimal = true;
                    solution.IsFeasible = true;
                    ExtractSolution(model, basis, xB, solution);
                    return solution;
                }

                double[] enteringColumnValues = GetColumn(enteringColumn);
                double[] d = Multiply(bInverse, enteringColumnValues);

                int leavingRow = -1;
                double smallestRatio = double.PositiveInfinity;

                for (int i = 0; i < constraintCount; i++)
                {
                    if (MathUtilities.IsPositive(d[i]))
                    {
                        double ratio = xB[i] / d[i];
                        if (ratio < smallestRatio)
                        {
                            smallestRatio = ratio;
                            leavingRow = i;
                        }
                    }
                }

                if (leavingRow == -1)
                {
                    solution.Iterations.Add(
                        CreateIterationOutput(
                            iteration, basis, bInverse, xB, y,
                            reducedCosts, enteringColumn, -1, d));
                    solution.IsUnbounded = true;
                    solution.IsFeasible = false;
                    solution.Iterations.Add("PROBLEM IS UNBOUNDED.");
                    return solution;
                }

                solution.Iterations.Add(
                    CreateIterationOutput(
                        iteration, basis, bInverse, xB, y,
                        reducedCosts, enteringColumn, leavingRow, d));

                // Product Form update: fold the eta vector into B^-1.
                double[] eta = new double[constraintCount];
                for (int i = 0; i < constraintCount; i++)
                {
                    eta[i] = i == leavingRow
                        ? 1.0 / d[i]
                        : -d[i] / d[leavingRow];
                }

                bInverse = ApplyEta(bInverse, eta, leavingRow);
                basis[leavingRow] = enteringColumn;
                iteration++;
            }
        }

        private void Validate(LPModel model)
        {
            if (!model.IsMaximization())
            {
                throw new NotSupportedException(
                    "Revised Simplex currently requires a maximization problem.");
            }

            foreach (Variable variable in model.Variables)
            {
                if (variable.Restriction != "+" &&
                    variable.Restriction != "int" &&
                    variable.Restriction != "bin")
                {
                    throw new NotSupportedException(
                        "Revised Simplex requires non-negative variables " +
                        "(unrestricted-in-sign variables are not yet supported).");
                }
            }
        }

        private void BuildStandardForm(LPModel model)
        {
            variableCount = model.Variables.Count;
            constraintCount = model.Constraints.Count;

            columnNames = new List<string>();
            List<double> costList = new();
            specialColumns = new Dictionary<int, (int row, double value)>();
            artificialColumns = new List<int>();

            for (int j = 0; j < variableCount; j++)
            {
                costList.Add(model.Variables[j].GetSignedObjectiveCoefficient());
                columnNames.Add(model.Variables[j].Name);
            }

            // Normalize every row so its RHS is >= 0 (flip sign + relation
            // if needed), then attach slack / surplus+artificial / artificial
            // columns depending on the (possibly flipped) relation.
            double[][] normalizedCoefficients = new double[constraintCount][];
            string[] normalizedRelations = new string[constraintCount];
            b = new double[constraintCount];

            for (int i = 0; i < constraintCount; i++)
            {
                Constraint constraint = model.Constraints[i];
                double[] coefficients = constraint.GetSignedCoefficients().ToArray();
                string relation = constraint.Relation;
                double rhs = constraint.RightHandSide;

                if (rhs < 0)
                {
                    for (int j = 0; j < coefficients.Length; j++)
                    {
                        coefficients[j] = -coefficients[j];
                    }
                    rhs = -rhs;
                    relation = relation switch
                    {
                        "<=" => ">=",
                        ">=" => "<=",
                        _ => relation
                    };
                }

                normalizedCoefficients[i] = coefficients;
                normalizedRelations[i] = relation;
                b[i] = rhs;
            }

            for (int i = 0; i < constraintCount; i++)
            {
                string relation = normalizedRelations[i];

                if (relation == "<=")
                {
                    int slackIndex = columnNames.Count;
                    columnNames.Add("s" + (i + 1));
                    costList.Add(0);
                    specialColumns[slackIndex] = (i, 1);
                }
                else if (relation == ">=")
                {
                    int surplusIndex = columnNames.Count;
                    columnNames.Add("e" + (i + 1));
                    costList.Add(0);
                    specialColumns[surplusIndex] = (i, -1);

                    int artificialIndex = columnNames.Count;
                    columnNames.Add("a" + (i + 1));
                    costList.Add(-BigM);
                    specialColumns[artificialIndex] = (i, 1);
                    artificialColumns.Add(artificialIndex);
                }
                else // "="
                {
                    int artificialIndex = columnNames.Count;
                    columnNames.Add("a" + (i + 1));
                    costList.Add(-BigM);
                    specialColumns[artificialIndex] = (i, 1);
                    artificialColumns.Add(artificialIndex);
                }
            }

            totalColumns = columnNames.Count;
            c = costList.ToArray();

            A = new double[constraintCount, variableCount];
            for (int i = 0; i < constraintCount; i++)
            {
                for (int j = 0; j < variableCount; j++)
                {
                    A[i, j] = normalizedCoefficients[i][j];
                }
            }
        }

        // The initial basic variable for a row is its artificial variable
        // if one was added (>= or =), otherwise its slack (<=).
        private int FindInitialBasicColumn(int row)
        {
            foreach (KeyValuePair<int, (int row, double value)> entry in specialColumns)
            {
                if (entry.Value.row == row &&
                    entry.Value.value == 1 &&
                    artificialColumns.Contains(entry.Key))
                {
                    return entry.Key;
                }
            }

            foreach (KeyValuePair<int, (int row, double value)> entry in specialColumns)
            {
                if (entry.Value.row == row && entry.Value.value == 1)
                {
                    return entry.Key;
                }
            }

            throw new InvalidOperationException(
                "No basic column found for row " + row + ".");
        }

        private int FindInfeasibleArtificialRow(int[] basis, double[] xB)
        {
            for (int i = 0; i < constraintCount; i++)
            {
                if (artificialColumns.Contains(basis[i]) &&
                    MathUtilities.IsPositive(xB[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private double[] GetColumn(int j)
        {
            double[] column = new double[constraintCount];

            if (j < variableCount)
            {
                for (int i = 0; i < constraintCount; i++)
                {
                    column[i] = A[i, j];
                }
            }
            else if (specialColumns.TryGetValue(j, out var entry))
            {
                column[entry.row] = entry.value;
            }

            return column;
        }

        private double[,] Identity(int size)
        {
            double[,] result = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                result[i, i] = 1;
            }
            return result;
        }

        private double[] Multiply(double[,] matrix, double[] vector)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            double[] result = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                double sum = 0;
                for (int j = 0; j < columns; j++)
                {
                    sum += matrix[i, j] * vector[j];
                }
                result[i] = sum;
            }
            return result;
        }

        private double[] MultiplyRowVector(double[] rowVector, double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            double[] result = new double[columns];

            for (int j = 0; j < columns; j++)
            {
                double sum = 0;
                for (int i = 0; i < rows; i++)
                {
                    sum += rowVector[i] * matrix[i, j];
                }
                result[j] = sum;
            }
            return result;
        }

        private double DotProduct(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }
            return sum;
        }

        // Applies the eta vector to B^-1 -- this IS the Product Form of
        // the Inverse update: newB^-1 = E * oldB^-1, without ever
        // recomputing the inverse from scratch.
        private double[,] ApplyEta(double[,] bInverse, double[] eta, int pivotRow)
        {
            int size = bInverse.GetLength(0);
            double[,] result = new double[size, size];
            double[] pivotRowValues = new double[size];

            for (int j = 0; j < size; j++)
            {
                pivotRowValues[j] = bInverse[pivotRow, j];
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    double baseValue = i == pivotRow ? 0 : bInverse[i, j];
                    result[i, j] = baseValue + eta[i] * pivotRowValues[j];
                }
            }

            return result;
        }

        private string CreateIterationOutput(
            int iteration,
            int[] basis,
            double[,] bInverse,
            double[] xB,
            double[] y,
            double[] reducedCosts,
            int enteringColumn,
            int leavingRow,
            double[]? d)
        {
            StringBuilder output = new();
            output.AppendLine(
                $"========== REVISED SIMPLEX ITERATION {iteration} ==========");
            output.AppendLine();

            output.AppendLine(
                "Basis: " + string.Join(", ", basis.Select(j => columnNames[j])));
            output.AppendLine();

            output.AppendLine("B-inverse (Product Form):");
            int size = bInverse.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                StringBuilder row = new();
                for (int j = 0; j < size; j++)
                {
                    row.Append(
                        MathUtilities.Round(bInverse[i, j])
                            .ToString("0.###")
                            .PadLeft(10));
                }
                output.AppendLine(row.ToString());
            }
            output.AppendLine();

            output.AppendLine("Basic solution (x_B = B^-1 * b):");
            for (int i = 0; i < size; i++)
            {
                output.AppendLine(
                    $"  {columnNames[basis[i]]} = " +
                    $"{MathUtilities.Round(xB[i]):0.###}");
            }
            output.AppendLine();

            output.AppendLine("Simplex multipliers (y = c_B * B^-1):");
            output.AppendLine(
                "  [" + string.Join(", ",
                    y.Select(v => MathUtilities.Round(v).ToString("0.###"))) + "]");
            output.AppendLine();

            output.AppendLine("Price-out (reduced costs, non-basic columns):");
            for (int j = 0; j < totalColumns; j++)
            {
                if (basis.Contains(j))
                {
                    continue;
                }
                string artificialNote = artificialColumns.Contains(j)
                    ? " (artificial, Big-M penalized)"
                    : "";
                output.AppendLine(
                    $"  {columnNames[j]}: c_bar = " +
                    $"{MathUtilities.Round(reducedCosts[j]):0.###}{artificialNote}");
            }
            output.AppendLine();

            if (enteringColumn == -1)
            {
                output.AppendLine(
                    "All reduced costs <= 0 -> current solution is optimal " +
                    "(pending an artificial-variable feasibility check).");
                return output.ToString();
            }

            output.AppendLine($"Entering variable: {columnNames[enteringColumn]}");

            if (d != null)
            {
                output.AppendLine(
                    "Transformed entering column (d = B^-1 * A_entering): " +
                    "[" + string.Join(", ",
                        d.Select(v => MathUtilities.Round(v).ToString("0.###"))) + "]");
            }

            output.AppendLine(
                leavingRow == -1
                    ? "No positive entry in d -> problem is unbounded."
                    : $"Leaving variable: {columnNames[basis[leavingRow]]}");

            return output.ToString();
        }

        private void ExtractSolution(LPModel model, int[] basis, double[] xB, Solution solution)
        {
            double objectiveValue = 0;
            for (int i = 0; i < constraintCount; i++)
            {
                // Artificials should be at 0 here (feasibility already
                // checked), so they don't distort the true objective value.
                if (!artificialColumns.Contains(basis[i]))
                {
                    objectiveValue += c[basis[i]] * xB[i];
                }
            }
            solution.ObjectiveValue = MathUtilities.Round(objectiveValue);

            foreach (Variable variable in model.Variables)
            {
                int column = columnNames.IndexOf(variable.Name);
                double value = 0;

                for (int i = 0; i < constraintCount; i++)
                {
                    if (basis[i] == column)
                    {
                        value = xB[i];
                        break;
                    }
                }

                solution.VariableValues[variable.Name] = MathUtilities.Round(value);
            }
        }
    }
}