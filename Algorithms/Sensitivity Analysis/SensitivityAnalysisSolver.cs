using Linear_Programming_381.Core;
using Linear_Programming_381.Models;
using Linear_Programming_381.Algorithms.Primal_Simplex;

namespace Linear_Programming_381.Algorithms.Sensitivity_Analysis
{
    public class SensitivityAnalysisSolver
    {
        private const double Epsilon = 0.000001;


        public void ValidateForSensitivity(LPModel model,Solution solution,Tableau tableau)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (solution == null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (tableau == null)
            {
                throw new ArgumentNullException(nameof(tableau));
            }

            if (!solution.IsOptimal)
            {
                throw new InvalidOperationException("Sensitivity analysis requires an optimal solution.");
            }

            if (!solution.IsFeasible)
            {
                throw new InvalidOperationException("Sensitivity analysis requires a feasible solution.");
            }

            foreach (Constraint constraint in model.Constraints)
            {
                if (constraint.Relation != "<=" &&constraint.Relation != ">=")
                {
                    throw new NotSupportedException("Sensitivity analysis currently supports <= and >= constraints.");
                }
            }
        }

        public List<string> GetBasicDecisionVariables(LPModel model, Tableau tableau)
        {
            List<string> result = new();

            int objectiveRow = tableau.RowCount - 1;

            foreach (Variable variable in model.Variables)
            {
                for (int i = 0; i < objectiveRow; i++)
                {
                    if (tableau.BasicVariables[i] == variable.Name)
                    {
                        result.Add(variable.Name);
                        break;
                    }
                }
            }

            return result;
        }


        public List<string> GetNonBasicDecisionVariables(LPModel model, Tableau tableau)
        {
            List<string> basicVariables = GetBasicDecisionVariables(model, tableau);

            return model.Variables.Where(variable => !basicVariables.Contains(variable.Name))
                .Select(variable => variable.Name).ToList();
        }

        public SensitivityRange GetNonBasicVariableRange(LPModel model, Tableau tableau, string variableName)
        {
            Variable? variable = model.Variables.FirstOrDefault(v => v.Name == variableName);

            if (variable == null)
            {
                throw new ArgumentException($"Variable {variableName} does not exist.");
            }

            int objectiveRow = tableau.RowCount - 1;

            if (tableau.BasicVariables.Contains(variableName))
            {
                throw new InvalidOperationException($"{variableName} is a basic variable.");
            }

            int column = tableau.ColumnNames.IndexOf(variableName);

            if (column == -1)
            {
                throw new InvalidOperationException($"Column for {variableName} could not be found.");
            }

            double currentCoefficient = variable.GetSignedObjectiveCoefficient();

            double reducedCost = tableau.Values[objectiveRow, column];

            if (Math.Abs(reducedCost) < Epsilon)
            {
                reducedCost = 0;
            }

            double allowableIncrease = Math.Max(0, reducedCost);

            return new SensitivityRange
            {
                ItemName = variableName,
                CurrentValue = currentCoefficient,

                MinimumValue = double.NegativeInfinity,

                MaximumValue = currentCoefficient + allowableIncrease,

                AllowableDecrease = double.PositiveInfinity,

                AllowableIncrease = allowableIncrease,

                Description = $"Objective coefficient range for non-basic variable {variableName}."
            };
        }

        public void ApplyNonBasicVariableCoefficientChange(LPModel model, Tableau tableau, string variableName, double newCoefficient)
        {
            SensitivityRange range = GetNonBasicVariableRange(model, tableau, variableName);

            if (newCoefficient < range.MinimumValue - Epsilon || newCoefficient > range.MaximumValue + Epsilon)
            {
                throw new InvalidOperationException(
                    $"The new coefficient {newCoefficient:0.###} is outside the allowable range " +
                    $"[{FormatBound(range.MinimumValue)}, " +
                    $"{FormatBound(range.MaximumValue)}].");
            }

            Variable? variable = model.Variables.FirstOrDefault(v => v.Name == variableName);

            if (variable == null)
            {
                throw new ArgumentException($"Variable {variableName} does not exist.");
            }

            double oldCoefficient = variable.GetSignedObjectiveCoefficient();

            double difference = newCoefficient - oldCoefficient;

            int column = tableau.ColumnNames.IndexOf(variableName);

            int objectiveRow = tableau.RowCount - 1;

            tableau.Values[objectiveRow, column] -= difference;

            variable.ObjectiveSign = newCoefficient < 0 ? '-' : '+';

            variable.ObjectiveCoefficient = Math.Abs(newCoefficient);
        }

        private string FormatBound(double value)
        {
            if (double.IsPositiveInfinity(value))
            {
                return "+Infinity";
            }

            if (double.IsNegativeInfinity(value))
            {
                return "-Infinity";
            }

            return value.ToString("0.###");
        }

        public SensitivityRange GetBasicVariableRange(LPModel model,Tableau tableau,string variableName)
        {
            Variable? variable =model.Variables.FirstOrDefault(v => v.Name == variableName);

            if (variable == null)
            {
                throw new ArgumentException($"Variable {variableName} does not exist.");
            }

            int basicRow =tableau.BasicVariables.IndexOf(variableName);

            if (basicRow == -1)
            {
                throw new InvalidOperationException($"{variableName} is not a basic variable.");
            }

            double currentCoefficient = variable.GetSignedObjectiveCoefficient();

            double minimumDelta = double.NegativeInfinity;

            double maximumDelta = double.PositiveInfinity;

            int objectiveRow = tableau.RowCount - 1;

            for (int column = 0;column < tableau.ColumnCount - 1;column++)
            {
                string columnName = tableau.ColumnNames[column];

                // Only test non-basic columns.
                if (tableau.BasicVariables.Contains(columnName))
                {
                    continue;
                }

                if(tableau.ArtificialVariables.Contains(columnName))
                {
                    continue;
                }

                double reducedCost = tableau.Values[objectiveRow,column];

                double rowCoefficient = tableau.Values[basicRow,column];

                if (Math.Abs(rowCoefficient) < Epsilon)
                {
                    continue;
                }

                double bound =-reducedCost /rowCoefficient;

                if (rowCoefficient > 0)
                {
                    minimumDelta = Math.Max(minimumDelta,bound);
                }
                else
                {
                    maximumDelta = Math.Min(maximumDelta,bound);
                }
            }

            double minimumValue = double.IsNegativeInfinity(minimumDelta)? double.NegativeInfinity: currentCoefficient +minimumDelta;

            double maximumValue = double.IsPositiveInfinity(maximumDelta)? double.PositiveInfinity: currentCoefficient +maximumDelta;

            double allowableDecrease = double.IsNegativeInfinity(minimumDelta)? double.PositiveInfinity: Math.Max(0,-minimumDelta);

            double allowableIncrease = double.IsPositiveInfinity(maximumDelta)? double.PositiveInfinity: Math.Max(0,maximumDelta);

            return new SensitivityRange
            {
                ItemName = variableName,
                CurrentValue = currentCoefficient,
                MinimumValue = minimumValue,
                MaximumValue = maximumValue,
                AllowableDecrease =
                    allowableDecrease,
                AllowableIncrease =
                    allowableIncrease,
                Description =
                    $"Objective coefficient range for basic variable {variableName}."
            };
        }

        public void ApplyBasicVariableCoefficientChange(LPModel model,Tableau tableau,string variableName,double newCoefficient)
        {
            SensitivityRange range =GetBasicVariableRange(model,tableau,variableName);

            if (newCoefficient <range.MinimumValue - Epsilon ||newCoefficient >range.MaximumValue + Epsilon)
            {
                throw new InvalidOperationException(
                    $"The new coefficient {newCoefficient:0.###} is outside the allowable range " +
                    $"[{FormatBound(range.MinimumValue)}, {FormatBound(range.MaximumValue)}].");
            }

            Variable? variable =model.Variables.FirstOrDefault(v => v.Name == variableName);

            if (variable == null)
            {
                throw new ArgumentException($"Variable {variableName} does not exist.");
            }

            int basicRow =tableau.BasicVariables.IndexOf(variableName);

            if (basicRow == -1)
            {
                throw new InvalidOperationException($"{variableName} is not basic.");
            }

            double oldCoefficient =variable.GetSignedObjectiveCoefficient();

            double difference =newCoefficient -oldCoefficient;

            int objectiveRow =tableau.RowCount - 1;

            for (int column = 0;column < tableau.ColumnCount;column++)
            {
                tableau.Values[objectiveRow,column] +=difference *tableau.Values[basicRow,column];
            }

            variable.ObjectiveSign =newCoefficient < 0? '-': '+';

            variable.ObjectiveCoefficient =Math.Abs(newCoefficient);
        }

        public SensitivityRange GetConstraintRhsRange(LPModel model,Tableau tableau,int constraintIndex)
        {
            if (constraintIndex < 0 ||constraintIndex >= model.Constraints.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(constraintIndex),"Invalid constraint index.");
            }

            Constraint constraint =model.Constraints[constraintIndex];

            var sensitivityColumn = GetConstraintSensitivityColumn(model,tableau,constraintIndex);

            int column =sensitivityColumn.Column;

            double multiplier =sensitivityColumn.Multiplier;

            int objectiveRow =tableau.RowCount - 1;

            int rhsColumn =tableau.ColumnCount - 1;

            double minimumDelta =double.NegativeInfinity;

            double maximumDelta =double.PositiveInfinity;

            /*
              xB(new) =
             
              xB(current)
              +
              delta * B^-1 e_i
             
              For <=:
              B^-1 e_i = slack column.
             
              For >=:
              B^-1 e_i = -excess column.
             */
            for (int row = 0;row < objectiveRow;row++)
            {
                double currentBasicValue =tableau.Values[row,rhsColumn];

                double effect =multiplier *tableau.Values[row,column];

                if (Math.Abs(effect) < Epsilon)
                {
                    continue;
                }

                double bound =-currentBasicValue /effect;

                if (effect > 0)
                {
                    minimumDelta =Math.Max(minimumDelta,bound);
                }
                else
                {
                    maximumDelta =Math.Min(maximumDelta,bound);
                }
            }

            double currentValue =constraint.RightHandSide;

            double minimumValue = double.IsNegativeInfinity(minimumDelta)? double.NegativeInfinity: currentValue +minimumDelta;

            double maximumValue =double.IsPositiveInfinity(maximumDelta)? double.PositiveInfinity: currentValue +maximumDelta;

            double allowableDecrease =double.IsNegativeInfinity(minimumDelta)? double.PositiveInfinity: Math.Max(0,-minimumDelta);

            double allowableIncrease =double.IsPositiveInfinity(maximumDelta)? double.PositiveInfinity: Math.Max(0,maximumDelta);

            return new SensitivityRange
            {
                ItemName =$"Constraint {constraintIndex + 1} RHS",

                CurrentValue =currentValue,

                MinimumValue =minimumValue,

                MaximumValue =maximumValue,

                AllowableDecrease =allowableDecrease,

                AllowableIncrease =allowableIncrease,

                Description =$"RHS range for constraint {constraintIndex + 1} ({constraint.Relation})."
            };
        }

        public void ApplyConstraintRhsChange(LPModel model,Tableau tableau,int constraintIndex,double newRhs)
        {
            SensitivityRange range =GetConstraintRhsRange(model,tableau,constraintIndex);

            if (newRhs <range.MinimumValue - Epsilon ||newRhs >range.MaximumValue + Epsilon)
            {
                throw new InvalidOperationException(
                    $"The new RHS value " +
                    $"{newRhs:0.###} is outside " +
                    $"the allowable range " +
                    $"[{FormatBound(range.MinimumValue)}, " +
                    $"{FormatBound(range.MaximumValue)}].");
            }

            Constraint constraint =model.Constraints[constraintIndex];

            double oldRhs =constraint.RightHandSide;

            double difference =newRhs -oldRhs;

            var sensitivityColumn =GetConstraintSensitivityColumn(model,tableau,constraintIndex);

            int column =sensitivityColumn.Column;

            double multiplier =sensitivityColumn.Multiplier;

            int rhsColumn =tableau.ColumnCount - 1;

            /*
             * Apply:
             *
             * RHS(new) =
             * RHS(old) +
             * delta * B^-1 e_i
             */
            for (int row = 0;row < tableau.RowCount;row++)
            {
                double effect =multiplier *tableau.Values[row,column];

                tableau.Values[row,rhsColumn] +=difference *effect;
            }

            constraint.RightHandSide =newRhs;
        }

        private (int Column, double Multiplier, string VariableName)
    GetConstraintSensitivityColumn(LPModel model,Tableau tableau,int constraintIndex)
        {
            if (constraintIndex < 0 ||constraintIndex >= model.Constraints.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(constraintIndex),"Invalid constraint index.");
            }

            int slackCounter = 0;
            int excessCounter = 0;

            for (int i = 0;i <= constraintIndex;i++)
            {
                Constraint constraint =model.Constraints[i];

                if (constraint.Relation == "<=")
                {
                    slackCounter++;

                    if (i == constraintIndex)
                    {
                        string variableName = $"s{slackCounter}";

                        int column =tableau.ColumnNames.IndexOf(variableName);

                        if (column == -1)
                        {
                            throw new InvalidOperationException($"Could not find {variableName} in the final tableau.");
                        }

                        /*
                         * Slack column initially contains +1,
                         * so it directly represents B^-1 e_i.
                         */
                        return (column,1.0,variableName);
                    }
                }
                else if (constraint.Relation == ">=")
                {
                    excessCounter++;

                    if (i == constraintIndex)
                    {
                        string variableName =$"e{excessCounter}";

                        int column =tableau.ColumnNames.IndexOf(variableName);

                        if (column == -1)
                        {
                            throw new InvalidOperationException($"Could not find {variableName} in the final tableau.");
                        }

                        /*
                          Excess column initially contains -1, therefore:
                         
                          excess column = -B^-1 e_i
                         
                          Multiply by -1 to recover B^-1 e_i.
                         */
                        return (column,-1.0,variableName);
                    }
                }
                else
                {
                    if (i == constraintIndex)
                    {
                        throw new NotSupportedException("Sensitivity analysis for equality constraints has not been implemented yet.");
                    }
                }
            }

            throw new InvalidOperationException("Could not determine the sensitivity column.");
        }

        public double GetShadowPrice(LPModel model,Tableau tableau,int constraintIndex)
        {
            var sensitivityColumn =GetConstraintSensitivityColumn(model,tableau,constraintIndex);

            int objectiveRow =tableau.RowCount - 1;

            double shadowPrice =sensitivityColumn.Multiplier *tableau.Values[objectiveRow,sensitivityColumn.Column];

            if (Math.Abs(shadowPrice) < Epsilon)
            {
                shadowPrice = 0;
            }

            return shadowPrice;
        }

        public Dictionary<string, double> GetShadowPrices(LPModel model,Tableau tableau)
        {
            Dictionary<string, double> shadowPrices = new();

            for (int i = 0;i < model.Constraints.Count;i++)
            {
                shadowPrices[$"Constraint {i + 1}"] =GetShadowPrice(model,tableau,i);
            }

            return shadowPrices;
        }

        public TechnologicalCoefficientRange GetNonBasicColumnCoefficientRange(LPModel model,Tableau tableau,string variableName,int constraintIndex)
        {
            if (constraintIndex < 0 ||constraintIndex >= model.Constraints.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(constraintIndex),"Invalid constraint index.");
            }

            Variable? variable =model.Variables.FirstOrDefault(v => v.Name == variableName);

            if (variable == null)
            {
                throw new ArgumentException($"Variable {variableName} does not exist.");
            }

            if (tableau.BasicVariables.Contains(variableName))
            {
                throw new InvalidOperationException($"{variableName} must be a non-basic variable.");
            }

            int variableColumn =tableau.ColumnNames.IndexOf(variableName);

            if (variableColumn == -1)
            {
                throw new InvalidOperationException($"Could not find column {variableName}.");
            }

            Constraint constraint =model.Constraints[constraintIndex];

            List<double> signedCoefficients =constraint.GetSignedCoefficients();

            int variableIndex =model.Variables.FindIndex(v => v.Name == variableName);

            double currentCoefficient =signedCoefficients[variableIndex];

            int objectiveRow =tableau.RowCount - 1;

            double currentReducedCost =tableau.Values[objectiveRow,variableColumn];

            double shadowPrice =GetShadowPrice(model,tableau,constraintIndex);

            double minimumDelta =double.NegativeInfinity;

            double maximumDelta =double.PositiveInfinity;

            if (Math.Abs(shadowPrice) > Epsilon)
            {
                double bound =-currentReducedCost /shadowPrice;

                if (shadowPrice > 0)
                {
                    minimumDelta = bound;
                }
                else
                {
                    maximumDelta = bound;
                }
            }

            double minimumValue =double.IsNegativeInfinity(minimumDelta)? double.NegativeInfinity: currentCoefficient +minimumDelta;

            double maximumValue =double.IsPositiveInfinity(maximumDelta)? double.PositiveInfinity: currentCoefficient +maximumDelta;

            double allowableDecrease =double.IsNegativeInfinity(minimumDelta)? double.PositiveInfinity: Math.Max(0,-minimumDelta);

            double allowableIncrease =double.IsPositiveInfinity(maximumDelta)? double.PositiveInfinity: Math.Max(0,maximumDelta);

            return new TechnologicalCoefficientRange
            {
                VariableName =variableName,

                ConstraintIndex =constraintIndex,

                CurrentValue =currentCoefficient,

                MinimumValue =minimumValue,

                MaximumValue =maximumValue,

                AllowableDecrease =allowableDecrease,

                AllowableIncrease =allowableIncrease
            };
        }

        public void ApplyNonBasicColumnCoefficientChange(LPModel model,Tableau tableau,string variableName,int constraintIndex,double newCoefficient)
        {
            TechnologicalCoefficientRange range =GetNonBasicColumnCoefficientRange(model,tableau,variableName,constraintIndex);

            if (newCoefficient <range.MinimumValue - Epsilon ||newCoefficient >range.MaximumValue + Epsilon)
            {
                throw new InvalidOperationException(
                    $"The new coefficient " +
                    $"{newCoefficient:0.###} is outside " +
                    $"the allowable range " +
                    $"[{FormatBound(range.MinimumValue)}, " +
                    $"{FormatBound(range.MaximumValue)}].");
            }

            int variableIndex =model.Variables.FindIndex(v => v.Name == variableName);

            Constraint constraint =model.Constraints[constraintIndex];

            double oldCoefficient =constraint.GetSignedCoefficients()[variableIndex];

            double difference =newCoefficient -oldCoefficient;

            int variableColumn =tableau.ColumnNames.IndexOf(variableName);

            if (variableColumn == -1)
            {
                throw new InvalidOperationException($"Could not find {variableName} in the tableau.");
            }

            var sensitivityColumn =GetConstraintSensitivityColumn(model,tableau,constraintIndex);

            for (int row = 0;row < tableau.RowCount;row++)
            {
                double effect =sensitivityColumn.Multiplier *tableau.Values[row,sensitivityColumn.Column];

                tableau.Values[row,variableColumn] +=difference *effect;
            }

            /*
             * Update the original model coefficient.
             */
            constraint.Signs[variableIndex] =newCoefficient < 0? '-': '+';

            constraint.Coefficients[variableIndex] =Math.Abs(newCoefficient);
        }

        public NewActivityResult AnalyseNewActivity(LPModel model,Tableau tableau,string variableName,double objectiveCoefficient,List<double> constraintCoefficients)
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException("The new activity must have a variable name.");
            }

            if (model.Variables.Any(v => v.Name.Equals(variableName,StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Variable {variableName} already exists.");
            }

            if (constraintCoefficients.Count !=model.Constraints.Count)
            {
                throw new ArgumentException("The new activity must contain one coefficient for every constraint.");
            }

            double zj = 0;

            for (int i = 0;i < model.Constraints.Count;i++)
            {
                double shadowPrice =GetShadowPrice(model,tableau,i);

                zj +=shadowPrice *constraintCoefficients[i];
            }

            double reducedCost =zj -objectiveCoefficient;

            if (Math.Abs(reducedCost) < Epsilon)
            {
                reducedCost = 0;
            }

            bool canImprove =reducedCost < -Epsilon;

            return new NewActivityResult
            {
                VariableName =variableName,

                ObjectiveCoefficient =objectiveCoefficient,

                ConstraintCoefficients =new List<double>(constraintCoefficients),

                ReducedCost =reducedCost,

                CanImproveSolution =canImprove
            };
        }

        public void AddNewActivityToModel(LPModel model,string variableName,double objectiveCoefficient,
            List<double> constraintCoefficients,string restriction = "+")
        {
            if (string.IsNullOrWhiteSpace(variableName))
            {
                throw new ArgumentException("The variable name cannot be empty.");
            }

            if (model.Variables.Any(v => v.Name.Equals(variableName,StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Variable {variableName} already exists.");
            }

            if (constraintCoefficients.Count !=model.Constraints.Count)
            {
                throw new ArgumentException("The number of coefficients must match the number of constraints.");
            }

            char objectiveSign =objectiveCoefficient < 0? '-': '+';

            Variable newVariable =new Variable(variableName,Math.Abs(objectiveCoefficient),
                objectiveSign,restriction);

            model.Variables.Add(newVariable);

            for (int i = 0;i < model.Constraints.Count;i++)
            {
                double coefficient =constraintCoefficients[i];

                char sign =coefficient < 0? '-': '+';

                model.Constraints[i].Coefficients.Add(Math.Abs(coefficient));

                model.Constraints[i].Signs.Add(sign);
            }
        }

        public (NewActivityResult Analysis,Tableau Tableau,Solution Solution)
            AddNewActivityAndResolve(LPModel model,Tableau currentTableau,string variableName,
            double objectiveCoefficient,List<double> constraintCoefficients)
        {
            NewActivityResult analysis =AnalyseNewActivity(model,currentTableau,variableName,
                    objectiveCoefficient,constraintCoefficients);

            
           //   Add it to the mathematical model.
             
            AddNewActivityToModel(model,variableName,objectiveCoefficient,constraintCoefficients);

            
           //   Rebuild the canonical tableau
             
            CanonicalConverter converter =
                new CanonicalConverter();

            Tableau newTableau =converter.Convert(model);

            PrimalSimplexSolver simplex =new PrimalSimplexSolver();

            Solution newSolution =simplex.Solve(model,newTableau);

            return (analysis,newTableau,newSolution);
        }

        public NewConstraintResult AnalyseNewConstraint(LPModel model,Solution solution,
            List<double> coefficients,string relation,double rightHandSide)
        {
            if (coefficients.Count !=model.Variables.Count)
            {
                throw new ArgumentException("The new constraint must contain one coefficient for every decision variable.");
            }

            if (relation != "<=" &&relation != ">=" &&relation != "=")
            {
                throw new ArgumentException("Constraint relation must be <=, >=, or =.");
            }

            double leftHandSide = 0;

            for (int i = 0;i < model.Variables.Count;i++)
            {
                string variableName =model.Variables[i].Name;

                double variableValue = 0;

                if (solution.VariableValues.ContainsKey(variableName))
                {
                    variableValue =solution.VariableValues[variableName];
                }

                leftHandSide +=coefficients[i] *variableValue;
            }

            bool satisfied;

            switch (relation)
            {
                case "<=":
                    satisfied =leftHandSide <=rightHandSide + Epsilon;
                    break;

                case ">=":
                    satisfied =leftHandSide >=rightHandSide - Epsilon;
                    break;

                case "=":
                    satisfied =Math.Abs(leftHandSide -rightHandSide)<= Epsilon;
                    break;

                default:
                    throw new InvalidOperationException(
                        "Unsupported constraint relation.");
            }

            return new NewConstraintResult
            {
                Coefficients =new List<double>(
                        coefficients),

                Relation =
                    relation,

                RightHandSide =
                    rightHandSide,

                LeftHandSideValue =
                    leftHandSide,

                CurrentSolutionSatisfiesConstraint =
                    satisfied
            };
        }

        public void AddNewConstraintToModel(LPModel model, List<double> coefficients, string relation, double rightHandSide)
        {
            if (coefficients.Count != model.Variables.Count)
            {
                throw new ArgumentException("The number of coefficients must match the number of decision variables.");
            }

            if (relation != "<=" && relation != ">=" && relation != "=")
            {
                throw new ArgumentException("Constraint relation must be <=, >=, or =.");
            }

            List<double> absoluteCoefficients = new();

            List<char> signs = new();

            foreach (double coefficient in coefficients)
            {
                absoluteCoefficients.Add(Math.Abs(coefficient));

                signs.Add(coefficient < 0 ? '-' : '+');
            }

            Constraint newConstraint = new Constraint(absoluteCoefficients, signs, relation, rightHandSide);

            model.Constraints.Add(newConstraint);
        }

        public (NewConstraintResult Analysis, Tableau Tableau,Solution Solution)
    AddNewConstraintAndResolve(LPModel model,Solution currentSolution,List<double> coefficients,string relation,double rightHandSide)
        {
            NewConstraintResult analysis =AnalyseNewConstraint(model,currentSolution,coefficients,relation,rightHandSide);

            AddNewConstraintToModel(model,coefficients,relation,rightHandSide);

            CanonicalConverter converter =new CanonicalConverter();

            Tableau newTableau =converter.Convert(model);

            PrimalSimplexSolver simplex =new PrimalSimplexSolver();

            Solution newSolution =simplex.Solve(model,newTableau);

            return (analysis,newTableau,newSolution);
        }
    }
}