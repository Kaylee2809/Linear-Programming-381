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
            LPModel workingModel = NormalizeModel(model);

            if (!workingModel.IsMaximization())
            {
                throw new NotSupportedException(
                    "Primal Simplex currently requires " +
                    "a maximization problem.");
            }
            foreach (Variable variable
                     in workingModel.Variables)
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

            int variableCount =
                workingModel.Variables.Count;
            int constraintCount =
                workingModel.Constraints.Count;

            int extraVariableCount = 0;

            foreach(Constraint constraint in workingModel.Constraints)
            {
                if(constraint.Relation == "<=")
                {
                    extraVariableCount++;
                }
                else if(constraint.Relation == ">=")
                {
                    extraVariableCount += 2;
                }
                else if(constraint.Relation == "=")
                {
                    extraVariableCount ++;
                }
                else
                {
                    throw new NotSupportedException(
                        $"Unsupported constraint relation: " +
                        $"{constraint.Relation}");
                }
            }
            int totalColumns =
                variableCount +
                extraVariableCount +
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
            List<string> artificialVariables = new();
            // Decision variable columns.
            foreach (Variable variable
                     in workingModel.Variables)
            {
                columnNames.Add(
                    variable.Name);
            }
            // Slack variable columns.
            int slackCount = 0;
            int excessCount = 0;
            int artificialCount = 0;

            foreach(Constraint constraint in workingModel.Constraints)
            {
                if(constraint.Relation == "<=")
                {
                    slackCount ++;

                    columnNames.Add($"s{slackCount}");
                }
                else if(constraint.Relation == ">=")
                {
                    excessCount++;
                    artificialCount++;

                    columnNames.Add($"e{excessCount}");

                    string artificialName = $"a{artificialCount}";

                    columnNames.Add(artificialName);

                    artificialVariables.Add(artificialName);
                }
                else if(constraint.Relation == "=")
                {
                    artificialCount++;

                    string artificialName = $"a{artificialCount}";

                    columnNames.Add(artificialName);
                    artificialVariables.Add(artificialName);
                }
            }

            // RHS.
            columnNames.Add("RHS");
            // Constraint rows.

            int currentExtraColumn = variableCount;
            for (int i = 0;
                 i < constraintCount;
                 i++)
            {
                Constraint constraint =
                    workingModel.Constraints[i];
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

                if(constraint.Relation == "<=")
                {
                    slackCount = GetSlackNumber(workingModel, i);

                    values[i, currentExtraColumn] = 1;

                    basicVariables.Add($"s{slackCount}");

                    currentExtraColumn++;
                }
                else if(constraint.Relation == ">=")
                {
                    excessCount = GetExcessNumber(workingModel, i);

                    artificialCount = GetArtificialNumber(workingModel, i);

                    //Excess variable
                    values[i, currentExtraColumn] = -1;

                    currentExtraColumn ++;

                    //Artificial Variable
                    values[i, currentExtraColumn] = 1;

                    string artificialName = $"a{artificialCount}";

                    basicVariables.Add(artificialName);

                    currentExtraColumn++;
                }else if(constraint.Relation == "=")
                {
                    artificialCount = GetArtificialNumber(workingModel, i);

                    values[i, currentExtraColumn] = 1;

                    string artificialName = $"a{artificialCount}";

                    basicVariables.Add(artificialName);

                    currentExtraColumn++;
                }
                // RHS.
                values[
                    i,
                    totalColumns - 1] =
                    constraint.RightHandSide;
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
                    -workingModel.Variables[j]
                        .GetSignedObjectiveCoefficient();
            }
            basicVariables.Add("Z");
            return new Tableau(
                values,
                columnNames,
                basicVariables,
                artificialVariables);
        }

        private int GetSlackNumber(LPModel model, int constraintIndex)
        {
            int count = 0;

            for(int i = 0; i <= constraintIndex; i++)
            {
                if(model.Constraints[i].Relation == "<=")
                {
                    count++;
                }
            }
            return count;
        }

        private int GetExcessNumber(LPModel model, int constraintIndex)
        {
            int count = 0;

            for(int i = 0; i <= constraintIndex; i++)
            {
                if(model.Constraints[i].Relation == ">=")
                {
                    count++;
                }
            }

            return count;
        }

        private int GetArtificialNumber(LPModel model, int constraintIndex)
        {
            int count = 0;

            for(int i = 0; i <= constraintIndex; i++)
            {
                if(model.Constraints[i].Relation == ">=" || model.Constraints[i].Relation == "=")
                {
                    count++;
                }
            }
            return count;
        }

        private LPModel NormalizeModel(LPModel model)
        {
            LPModel normalizedModel =
                new LPModel
                {
                    ObjectiveType =
                        model.ObjectiveType
                };


            foreach (Variable variable
                     in model.Variables)
            {
                normalizedModel.Variables.Add(
                    variable);
            }

            foreach (Constraint constraint
                     in model.Constraints)
            {

                if (constraint.RightHandSide >= 0)
                {
                    normalizedModel.Constraints.Add(
                        constraint);

                    continue;
                }


                List<double> originalCoefficients =
                    constraint.GetSignedCoefficients();

                List<double> newCoefficients =
                    new();

                List<char> newSigns =
                    new();

                foreach (double coefficient
                         in originalCoefficients)
                {
                    double newCoefficient =
                        -coefficient;

                    newCoefficients.Add(
                        Math.Abs(newCoefficient));

                    newSigns.Add(
                        newCoefficient < 0
                            ? '-'
                            : '+');
                }

                string newRelation =
                    constraint.Relation;

                if (newRelation == "<=")
                {
                    newRelation = ">=";
                }
                else if (newRelation == ">=")
                {
                    newRelation = "<=";
                }

                Constraint normalizedConstraint =
                    new Constraint(
                        newCoefficients,
                        newSigns,
                        newRelation,
                        -constraint.RightHandSide);

                normalizedModel.Constraints.Add(
                    normalizedConstraint);
            }

            return normalizedModel;
        }
    }
}
