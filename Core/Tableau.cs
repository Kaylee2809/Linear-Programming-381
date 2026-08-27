using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Core
{
    public class Tableau
    {
        public double[,] Values { get; set; }
        public List<string> ColumnNames { get; set; }
        public List<string> BasicVariables { get; set; }
        public List<string> ArtificialVariables{get;set;}
        public int RowCount =>
            Values.GetLength(0);
        public int ColumnCount =>
            Values.GetLength(1);
        public Tableau(
            double[,] values,
            List<string> columnNames,
            List<string> basicVariables,
            List<string>? artificialVariables = null)
        {
            Values = values;
            ColumnNames = columnNames;
            BasicVariables = basicVariables;
            ArtificialVariables = artificialVariables ?? new List<string>();
        }
        public void Pivot(
            int pivotRow,
            int pivotColumn)
        {
            double pivot =
                Values[
                    pivotRow,
                    pivotColumn];
            if (MathUtilities.IsZero(pivot))
            {
                throw new InvalidOperationException(
                    "Cannot pivot on a zero value.");
            }
            // Divide the pivot row by the pivot.
            for (int j = 0;
                 j < ColumnCount;
                 j++)
            {
                Values[
                    pivotRow,
                    j] /= pivot;
            }
            // Eliminate the pivot column from every other row.
            for (int i = 0;
                 i < RowCount;
                 i++)
            {
                if (i == pivotRow)
                {
                    continue;
                }
                double factor =
                    Values[
                        i,
                        pivotColumn];
                for (int j = 0;
                     j < ColumnCount;
                     j++)
                {
                    Values[
                        i,
                        j] -=
                        factor *
                        Values[
                            pivotRow,
                            j];
                }
            }
            BasicVariables[pivotRow] =
                ColumnNames[pivotColumn];
        }
        public string ToFormattedString()
        {
            StringBuilder output = new();
            const int width = 12;
            output.Append(
                "BV".PadRight(width));
            foreach (string column
                     in ColumnNames)
            {
                output.Append(
                    column.PadLeft(width));
            }
            output.AppendLine();
            for (int i = 0;
                 i < RowCount;
                 i++)
            {
                output.Append(
                    BasicVariables[i]
                        .PadRight(width));

                for (int j = 0;
                     j < ColumnCount;
                     j++)
                {
                    output.Append(
                        Values[i, j]
                            .ToString("0.###")
                            .PadLeft(width));
                }
                output.AppendLine();
            }
            return output.ToString();
        }
    }
}
