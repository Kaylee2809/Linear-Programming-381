using Linear_Programming_381.Algorithms.Primal_Simplex;
using Linear_Programming_381.Core;
using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Linear_Programming_381.Algorithms.Branch_and_Bound_Simplex
{
    public class BranchandBoundSimplexSolver
    {
        private const double Tolerance = 0.000001;

        public BranchandBoundResult Solve(LPModel model)
        {
            BranchandBoundResult result = new BranchandBoundResult();
            
            Stack<BranchandBoundNode> openNodes = new Stack<BranchandBoundNode>();

            Solution? bestSolution = null;

            int? bestNodeId = null;

            int nextNodeId = 1;

            BranchandBoundNode rootNode = new BranchandBoundNode(
                nextNodeId++,
                null,
                cloneModel(model),
                "Root LP relaxation",
                0
            );

            openNodes.Push(rootNode);

            while(openNodes.Count > 0)
            {
                BranchandBoundNode node = openNodes.Pop();

                result.Nodes.Add(node);

                CanonicalConverter converter = new CanonicalConverter();

                Tableau tableau = converter.Convert(node.Model);

                PrimalSimplexSolver simplexSolver = new PrimalSimplexSolver();

                node.Solution = simplexSolver.Solve(node.Model, tableau);

                if (node.Solution.IsUnbounded)
                {
                    node.IsFathomed = true;

                    node.FathomReason = "LP Relaxation is Unbounded.";

                    AddNodeOutput(result, node);

                    continue;
                }

                if(node.Solution.IsInfeasible || !node.Solution.IsFeasible)
                {
                    node.IsFathomed = true;

                    node.FathomReason = "LP Relaxation is Infeasible";

                    AddNodeOutput(result, node);

                    continue;
                }

                if(CannotBeatBestCandidate(model, node.Solution, bestSolution))
                {
                    node.IsFathomed = true;

                    node.FathomReason = "Node bound cannot improve the current best candidate.";

                    AddNodeOutput(result, node);

                    continue;
                }

                if(IsIntegerSolution(model, node.Solution))
                {
                    if(IsBetterCandidate(model, node.Solution, bestSolution))
                    {
                        bestSolution = node.Solution;
                        bestNodeId = node.NodeId;
                    }

                    node.IsFathomed = true;

                    node.FathomReason = "Integer solution found.";

                    AddNodeOutput(result, node);

                    continue;
                }

                Variable? branchVariable = FindBranchVariable(model, node.Solution);

                if(branchVariable == null)
                {
                    node.IsFathomed = true;
                    node.FathomReason = "No valid branching variable found.";

                    AddNodeOutput(result, node);

                    continue;
                }

                node.BranchingVariable = branchVariable.Name;

                node.BranchingValue = node.Solution.VariableValues[branchVariable.Name];

                AddNodeOutput(result, node);

                CreateChildren(node, branchVariable, ref nextNodeId, openNodes);
            }

            result.BestNodeId = bestNodeId;

            if(bestSolution == null)
            {
                result.IsFeasible = false;
                result.IsOptimal = false;
                result.IsInfeasible = true;

                result.SearchTree = CreateSearchTreeOutput(result);

                result.Iterations.Add(result.SearchTree);

                result.Iterations.Add("========================================\n" +
                "NO INTEGER SOLUTION FOUND\n" + 
                "========================================");

                return result;
            }

            result.IsFeasible = true;
            result.IsOptimal = true;
            result.IsInfeasible = false;

            result.ObjectiveValue = bestSolution.ObjectiveValue;

            foreach(KeyValuePair<string, double> value in bestSolution.VariableValues)
            {
                result.VariableValues[value.Key] = value.Value;
            }

            result.SearchTree = CreateSearchTreeOutput(result);

            result.Iterations.Add(result.SearchTree);

            result.Iterations.Add(CreateBestCandidateOutput(model, result));

            return result;
        }

        private String CreateBestCandidateOutput(LPModel model, BranchandBoundResult result)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("========================================");

            output.AppendLine("BEST INTEGER CANDIDATE");

            output.AppendLine("========================================");

            if (result.BestNodeId.HasValue)
            {
                output.AppendLine($"Best Node: Node {result.BestNodeId.Value}");
            }

            output.AppendLine();

            foreach(Variable variable in model.Variables)
            {
                if (result.VariableValues.ContainsKey(variable.Name))
                {
                    output.AppendLine($"{variable.Name} = {result.VariableValues[variable.Name]:0.###}");
                }
            }

            output.AppendLine();

            output.AppendLine($"Optimal Objective Value = {result.ObjectiveValue:0.###}");

            output.AppendLine("========================================");

            return output.ToString();
        }

        private void AddNodeOutput(BranchandBoundResult result, BranchandBoundNode node)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("========================================");

            output.AppendLine($"BRANCH & BOUND NODE {node.NodeId}");

            output.AppendLine("========================================");

            if (node.ParentNodeId.HasValue)
            {
                output.AppendLine($"Parent Node: {node.ParentNodeId.Value}");
            }
            else
            {
                output.AppendLine("Parent Node: None");
            }

            output.AppendLine($"Depth: {node.Depth}");

            output.AppendLine($"Branch: {node.BranchDescription}");

            output.AppendLine();

            output.AppendLine("SUB-PROBLEM:");

            output.AppendLine(node.Model.GetObjectiveString());

            foreach(Constraint constraint in node.Model.Constraints)
            {
                output.AppendLine(node.Model.GetConstraintString(constraint));
            }

            output.AppendLine(node.Model.GetRestrictionsString());

            output.AppendLine();

            if(node.Solution != null)
            {
                foreach(string iteration in node.Solution.Iterations)
                {
                    output.AppendLine(iteration);
                }

                if(node.Solution.IsFeasible && !node.Solution.IsUnbounded)
                {
                    output.AppendLine("LP RELAXATION SOLUTION:");

                    foreach(Variable variable in node.Model.Variables)
                    {
                        if (node.Solution.VariableValues.ContainsKey(variable.Name))
                        {
                            output.AppendLine($"{variable.Name} = " +
                            $"{node.Solution.VariableValues[variable.Name]:0.###}");
                        }
                    }

                    output.AppendLine($"Z = " +
                    $"{node.Solution.ObjectiveValue:0.###}");

                    if(!string.IsNullOrWhiteSpace(node.BranchingVariable) && node.BranchingValue.HasValue)
                    {

                        double value = node.BranchingValue.Value;

                        output.AppendLine();

                        output.AppendLine($"BRANCHING ON: {node.BranchingVariable} = " +
                        $"{value:0.###}");

                        output.AppendLine($"Left Child: {node.BranchingVariable} <= {Math.Floor(value):0.###}");

                        output.AppendLine($"Right Child: {node.BranchingVariable} >= {Math.Ceiling(value):0.###}");
                    }
                }
            }

            if (node.IsFathomed)
            {
                output.AppendLine();

                output.AppendLine($"FATHOMED: {node.FathomReason}");
            }

            output.AppendLine();

            result.Iterations.Add(output.ToString());
        }
        private LPModel cloneModel(LPModel original)
        {
            LPModel clone = new LPModel();

            clone.ObjectiveType = original.ObjectiveType;

            foreach(Variable variable in original.Variables)
            {
                Variable copiedVariable = new Variable(variable.Name, variable.ObjectiveCoefficient,
                variable.ObjectiveSign, variable.Restriction);

                clone.Variables.Add(copiedVariable);
            }

            foreach(Constraint constraint in original.Constraints)
            {
                Constraint copiedConstraint = new Constraint(new List<double>(constraint.Coefficients),
                new List<char>(constraint.Signs),
                constraint.Relation,
                constraint.RightHandSide);

                clone.Constraints.Add(copiedConstraint);
            }
            return clone;
        }

        private bool RequiresIntegerValue(Variable variable)
        {
            return variable.IsInteger() || variable.IsBinary();
        }

        private bool IsIntegerValue(double value)
        {
            return Math.Abs(value - Math.Round(value)) <= Tolerance;
        }

        private bool IsIntegerSolution(LPModel model, Solution solution)
        {
            foreach(Variable variable in model.Variables)
            {
                if (!RequiresIntegerValue(variable))
                {
                    continue;
                }

                if (!solution.VariableValues.ContainsKey(variable.Name))
                {
                    return false;
                }

                double value = solution.VariableValues[variable.Name];

                if (variable.IsBinary())
                {
                    bool isZero = Math.Abs(value) <= Tolerance;

                    bool isOne = Math.Abs(value - 1) <= Tolerance;

                    if(!isZero && !isOne)
                    {
                        return false;
                    }
                }else if (!IsIntegerValue(value))
                {
                    return false;
                }
            }

            return true;
        }

        private Variable? FindBranchVariable(LPModel model, Solution solution)
        {
            foreach(Variable variable in model.Variables)
            {
                if (!RequiresIntegerValue(variable))
                {
                    continue;
                }

                if (!solution.VariableValues.ContainsKey(variable.Name))
                {
                    continue;
                }

                double value = solution.VariableValues[variable.Name];

                if (!IsIntegerValue(value))
                {
                    return variable;
                }
            }

            return null;
        }

        private Constraint CreateBranchConstraint(LPModel model, int variableIndex, string relation, double rightHandSide)
        {
            List<double> coefficients = new List<double>();

            List<char> signs = new List<char>();

            for(int i = 0; i < model.Variables.Count; i++)
            {
                if(i == variableIndex)
                {
                    coefficients.Add(1);
                }
                else
                {
                    coefficients.Add(0);
                }

                signs.Add('+');
            }

            return new Constraint(coefficients, signs, relation, rightHandSide);
        }

        private void CreateChildren(BranchandBoundNode parentNode, Variable branchVariable, 
        ref int nextNodeId, Stack<BranchandBoundNode> openNodes)
        {
            int variableIndex = parentNode.Model.Variables.FindIndex(variable => variable.Name == branchVariable.Name);

            if(variableIndex == -1)
            {
                throw new InvalidOperationException(
                    $"Variable {branchVariable.Name} was not found in the model."
                );
            }

            if(parentNode.Solution == null)
            {
                throw new InvalidOperationException(
                    "Cannot branch from a node that has not been solved."
                );
            }

            double value = parentNode.Solution.VariableValues[branchVariable.Name];

            double lowerBound = Math.Floor(value);

            double upperBound = Math.Ceiling(value);

            //Lower than child (xi <= lowerBound)

            LPModel leftModel = cloneModel(parentNode.Model);

            Constraint leftConstraint = CreateBranchConstraint(leftModel, variableIndex, "<=", lowerBound);

            leftModel.Constraints.Add(leftConstraint);

            BranchandBoundNode leftNode = new BranchandBoundNode(
                nextNodeId++,
                parentNode.NodeId, 
                leftModel,
                $"{branchVariable.Name} <= " + 
                $"{lowerBound:0.###}",
                parentNode.Depth + 1);

            //Upper than child (xi >= upperBound)
            LPModel rightModel = cloneModel(parentNode.Model);

            Constraint rightConstraint = CreateBranchConstraint(rightModel, variableIndex, ">=", upperBound);

            rightModel.Constraints.Add(rightConstraint);

            BranchandBoundNode rightNode = new BranchandBoundNode(
                nextNodeId++,
                parentNode.NodeId,
                rightModel,
                $"{branchVariable.Name} >= " +
                $"{upperBound:0.###}",
                parentNode.Depth + 1
            );

            //Since stack is LIFO, right child will be pushed first so that the left child is solved first

            openNodes.Push(rightNode);
            openNodes.Push(leftNode);
        }

        private bool IsBetterCandidate(LPModel model, Solution candidate, Solution? currentBest)
        {
            if(currentBest == null)
            {
                return true;
            }

            if (model.IsMaximization())
            {
                return candidate.ObjectiveValue > currentBest.ObjectiveValue + Tolerance;
            }

            return candidate.ObjectiveValue < currentBest.ObjectiveValue - Tolerance;
        }

        private bool CannotBeatBestCandidate(LPModel model, Solution nodeSolution, Solution? bestSolution)
        {
            if(bestSolution == null)
            {
                return false;
            }

            if (model.IsMaximization())
            {
                return nodeSolution.ObjectiveValue <= bestSolution.ObjectiveValue + Tolerance;
            }

            return nodeSolution.ObjectiveValue >= bestSolution.ObjectiveValue - Tolerance;
        }

        private string CreateSearchTreeOutput(BranchandBoundResult result)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("========================================");

            output.AppendLine("BRANCH & BOUND SEARCH TREE");

            output.AppendLine("========================================");

            output.AppendLine();

            foreach(BranchandBoundNode node in result.Nodes)
            {
                string indentation = new string(' ', node.Depth * 4);

                output.Append(indentation);

                output.Append($"Node {node.NodeId}");

                if (node.ParentNodeId.HasValue)
                {
                    output.Append($" [Parent: Node " + 
                    $"{node.ParentNodeId.Value}]");
                }
                else
                {
                    output.Append(" [ROOT]");
                }

                output.AppendLine();

                output.Append(indentation);

                output.AppendLine($" Branch: {node.BranchDescription}");

                if(node.Solution != null && node.Solution.IsFeasible && !node.Solution.IsUnbounded)
                {
                    output.Append(indentation);

                    output.AppendLine($" LP Bound: {node.Solution.ObjectiveValue:0.###}");
                }

                if(!string.IsNullOrWhiteSpace(node.BranchingVariable) && node.BranchingValue.HasValue)
                {
                    double value = node.BranchingValue.Value;

                    output.Append(indentation);

                    output.AppendLine($" Branched on: {node.BranchingVariable} = {value:0.###}");

                    output.Append(indentation);

                    output.AppendLine($"Children: {node.BranchingVariable} <= " +
                    $"{Math.Floor(value):0.###} | " +
                    $"{node.BranchingVariable} >= " +
                    $"{Math.Ceiling(value):0.###}");
                }

                if (node.IsFathomed)
                {
                    output.Append(indentation);

                    output.AppendLine($" FATHOMED: {node.FathomReason}");
                }

                if(result.BestNodeId.HasValue && result.BestNodeId.Value == node.NodeId)
                {
                    output.Append(indentation);

                    output.AppendLine(" *** BEST CANDIDATE ***");
                }

                output.AppendLine();
            }
            return output.ToString();
        }
    }
}