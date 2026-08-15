using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.File_Handling
{
    public class FileWriter
    {
        public void WriteToFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }
        public void Write(
            string filePath,
            LPModel model,
            Solution solution)
        {
            StringBuilder output = new();
            output.AppendLine(
                "========================================");
            output.AppendLine(
                "LP381 SOLVER OUTPUT");
            output.AppendLine(
                "========================================");
            output.AppendLine();
            output.AppendLine(
                "PROGRAMMING MODEL");
            output.AppendLine(
                model.GetObjectiveString());
            output.AppendLine();
            foreach (Constraint constraint
                     in model.Constraints)
            {
                output.AppendLine(
                    model.GetConstraintString(
                        constraint));
            }
            output.AppendLine();
            output.AppendLine(
                "Variable Restrictions:");
            output.AppendLine(
                model.GetRestrictionsString());
            output.AppendLine();
            output.AppendLine(
                "========================================");
            output.AppendLine(
                "TABLEAU ITERATIONS");
            output.AppendLine(
                "========================================");
            output.AppendLine();
            foreach (string iteration
                     in solution.Iterations)
            {
                output.AppendLine(iteration);
            }
            output.AppendLine();
            output.AppendLine(
                "========================================");
            output.AppendLine(
                "FINAL SOLUTION");
            output.AppendLine(
                "========================================");
            if (solution.IsInfeasible)
            {
                output.AppendLine(
                    "The model is INFEASIBLE.");
            }
            else if (solution.IsUnbounded)
            {
                output.AppendLine(
                    "The model is UNBOUNDED.");
            }
            else
            {
                output.AppendLine(
                    $"Objective Value = " +
                    $"{solution.ObjectiveValue:0.###}");
                foreach (var variable
                         in solution.VariableValues)
                {
                    output.AppendLine(
                        $"{variable.Key} = " +
                        $"{variable.Value:0.###}");
                }
            }
            string? directory =
                Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(
                filePath,
                output.ToString());
        }
    }
}
