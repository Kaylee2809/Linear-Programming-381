using Linear_Programming_381.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linear_Programming_381.Algorithms.Branch_and_Bound_Knapsack
{
    public class BranchAndBoundKnapsackSolver
    {
        private const double Tolerance = 0.000001;

        public BranchAndBoundKnapsackResult Solve(LPModel model)
        {

            ValidateModel(model);

            BranchAndBoundKnapsackResult result = new BranchAndBoundKnapsackResult();

            List<KnapsackItem> items = CreateItems(model);

            items = SortItems(items);

            double capacity = model.Constraints[0].RightHandSide;

            Stack<KnapsackNode> openNodes = new Stack<KnapsackNode>();

            KnapsackNode? bestNode = null;

            int nextNodeId = 1;

            KnapsackNode root = new KnapsackNode(nextNodeId++, null, 0, 0, 0, 0, new List<int>(), "Root");

            root.Bound = CalculateBound(root, items, capacity);

            openNodes.Push(root);

            while(openNodes.Count > 0)
            {
                KnapsackNode node = openNodes.Pop();

                result.Nodes.Add(node);

                //If capacity is exceeded
                if(node.CurrentWeight > capacity + Tolerance)
                {
                    node.IsFathomed = true;
                    node.FathomReason = "Capacity Exceeded.";

                    AddNodeOutput(result, node, items);

                    continue;
                }

                //Update best candidate if it is feasible
                if(IsBetterCandidate(node, bestNode))
                {
                    bestNode = node;
                }

                if(node.Level >= items.Count)
                {
                    node.IsFathomed = true;

                    node.FathomReason = "All items have been decided.";

                    AddNodeOutput(result, node, items);

                    continue;
                }

                //If bound cant beat best node
                if(bestNode != null && node.Bound <= bestNode.CurrentValue + Tolerance)
                {
                    node.IsFathomed = true;

                    node.FathomReason = "Upper bound cannot improve the best candidate.";

                    AddNodeOutput(result, node, items);

                    continue;
                }

                AddNodeOutput(result, node, items);

                KnapsackItem currentItem = items[node.Level];

                CreateChildren(node, currentItem, items, capacity, ref nextNodeId, openNodes);
            }

            if(bestNode == null)
            {
                result.IsFeasible = false;
                result.IsOptimal = false;
                result.IsInfeasible = true;

                return result;
            }

            result.IsFeasible = true;
            result.IsOptimal = true;
            result.IsInfeasible = false;

            result.BestNodeId = bestNode.NodeId;

            result.ObjectiveValue = bestNode.CurrentValue;

            SetFinalVariableValues(model, items, bestNode, result);

            result.SearchTree = CreateSearchTreeOutput(result);

            result.Iterations.Add(result.SearchTree);

            result.Iterations.Add(CreateBestCandidateOutput(model, result, bestNode));

            return result;
        }

        private void CreateChildren(KnapsackNode parent, KnapsackItem currentItem, List<KnapsackItem> items, 
        double capacity, ref int nextNodeId, Stack<KnapsackNode> openNodes)
        {
            List<int> includeDecisions = new List<int>(parent.Decisions);

            includeDecisions.Add(1);

            KnapsackNode includeNode = new KnapsackNode(nextNodeId++, parent.NodeId, parent.Depth + 1,
            parent.Level + 1, parent.CurrentValue + currentItem.Value, parent.CurrentWeight + currentItem.Weight, 
            includeDecisions, $"{currentItem.VariableName} = 1");

            includeNode.Bound = CalculateBound(includeNode, items, capacity);

            //Exclude current item
            List<int> excludeDecisions = new List<int>(parent.Decisions);

            excludeDecisions.Add(0);

            KnapsackNode excludeNode = new KnapsackNode(nextNodeId++, parent.NodeId, parent.Depth + 1,
            parent.Level + 1, parent.CurrentValue, parent.CurrentWeight, excludeDecisions,
            $"{currentItem.VariableName} = 0");

            excludeNode.Bound = CalculateBound(excludeNode, items, capacity);

            //Push exclude first so included variables are processed first in the stack

            openNodes.Push(excludeNode);
            openNodes.Push(includeNode);
        }

        private void AddNodeOutput(BranchAndBoundKnapsackResult result, KnapsackNode node, List<KnapsackItem> items)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("========================================");

            output.AppendLine($"KNAPSACK NODE {node.NodeId}");

            output.AppendLine("========================================");

            output.AppendLine($"Parent: {(node.ParentNodeId.HasValue ? "Node " + node.ParentNodeId.Value: "None ")}");

            output.AppendLine($"Branch: {node.BranchDescription}");

            output.AppendLine($"Current Value: {node.CurrentValue:0.###}");

            output.AppendLine($"Current Weight: {node.CurrentWeight:0.###}");

            output.AppendLine($"Upper Bound: {node.Bound:0.###}");

            output.AppendLine();
            output.AppendLine("Decisions:");

            for(int i = 0; i <node.Decisions.Count; i++)
            {
                output.AppendLine($"{items[i].VariableName} = {node.Decisions[i]}");
            }

            if (node.IsFathomed)
            {
                output.AppendLine();
                output.AppendLine($"FATHOMED: {node.FathomReason}");
            }

            output.AppendLine();

            result.Iterations.Add(output.ToString());
        }

        private void SetFinalVariableValues(LPModel model, List<KnapsackItem> items,
         KnapsackNode bestNode, BranchAndBoundKnapsackResult result)
        {
            foreach(Variable variable in model.Variables)
            {
                result.VariableValues[variable.Name] = 0;
            }

            for(int i = 0; i < bestNode.Decisions.Count; i++)
            {
                KnapsackItem item = items[i];

                result.VariableValues[item.VariableName] = bestNode.Decisions[i];
            }
        }

        private string CreateSearchTreeOutput(BranchAndBoundKnapsackResult result)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("========================================");

            output.AppendLine("KNAPSACK SEARCH TREE");

            output.AppendLine("========================================");

            output.AppendLine();

            foreach(KnapsackNode node in result.Nodes)
            {
                string indentation = new string(' ', node.Depth*4);

                output.AppendLine($"{indentation}Node {node.NodeId} [{node.BranchDescription}]");

                output.AppendLine($"{indentation} Value = {node.CurrentValue:0.###}");

                output.AppendLine($"{indentation} Weight = {node.CurrentWeight:0.###}");

                output.AppendLine($"{indentation} Bound = {node.Bound:0.###}");

                if (node.IsFathomed)
                {
                    output.AppendLine($"{indentation} FATHOMED: {node.FathomReason}");
                }

                if(result.BestNodeId.HasValue && result.BestNodeId.Value == node.NodeId)
                {
                    output.AppendLine($"{indentation} *** BEST CANDIDATE ***");
                }

                output.AppendLine();
            }

            return output.ToString();
        }

        private string CreateBestCandidateOutput(LPModel model, BranchAndBoundKnapsackResult result, KnapsackNode bestNode)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("========================================");

            output.AppendLine("KNAPSACK BEST CANDIDATE");

            output.AppendLine("========================================");

            output.AppendLine($"Best Node:Node {bestNode.NodeId}");

            output.AppendLine();

            foreach(Variable variable in model.Variables)
            {
                output.AppendLine($"{variable.Name} = {result.VariableValues[variable.Name]:0}");
            }

            output.AppendLine();

            output.AppendLine($"Total Weight = {bestNode.CurrentWeight:0.###}");

            output.AppendLine($"Optimal Objective Value = {result.ObjectiveValue:0.###}");

            output.AppendLine("========================================");

            return output.ToString();
        }

        private void ValidateModel(LPModel model)
        {
            if (!model.IsMaximization())
            {
                throw new InvalidOperationException("Knapsack currently only supports maximisation models only.");
            }

            if(model.Constraints.Count != 1)
            {
                throw new InvalidOperationException("Knapsack requires exactly one constraint only.");
            }

            Constraint constraint = model.Constraints[0];

            if(constraint.Relation != "<=")
            {
                throw new InvalidOperationException("Knapsack requires the constraint to be <=");
            }

            foreach(Variable variable in model.Variables)
            {
                if (!variable.IsBinary())
                {
                    throw new InvalidOperationException("Knapsack requires all variables to be binary");
                }
            }
        }

        private List<KnapsackItem> CreateItems(LPModel model)
        {
            Constraint constraint = model.Constraints[0];

            List<KnapsackItem> items = new List<KnapsackItem>();

            for(int i = 0; i < model.Variables.Count; i++)
            {
                Variable variable = model.Variables[i];

                double value = variable.GetSignedObjectiveCoefficient();

                double weight = constraint.Coefficients[i];

                if(constraint.Signs[i] == '-')
                {
                    weight *= -1;
                }

                if(weight < 0)
                {
                    throw new InvalidOperationException("The weights of the items must be positive");
                }

                KnapsackItem item = new KnapsackItem(variable.Name, value, weight, i);
                items.Add(item);
            }

            return items;
        }

        private List<KnapsackItem> SortItems(List<KnapsackItem> items)
        {
            return items.OrderByDescending(item => item.Ratio).ToList();
        }

        private double CalculateBound(KnapsackNode node, List<KnapsackItem> items, double capacity)
        {
            if(node.CurrentWeight > capacity + Tolerance)
            {
                return 0;
            }

            double bound = node.CurrentValue;

            double totalWeight = node.CurrentWeight;

            int index = node.Level;

            while(index < items.Count)
            {
                KnapsackItem item = items[index];

                if(totalWeight + item.Weight <= capacity)
                {
                    totalWeight+= item.Weight;

                    bound += item.Value;
                }
                else
                {
                    double remainingCapacity = capacity - totalWeight;

                    if(item.Weight > Tolerance)
                    {
                        double fraction = remainingCapacity/item.Weight;

                        bound += item.Value*fraction;
                    }
                    break;
                }
                index++;
            }
            return bound;
        }

        private bool IsBetterCandidate(KnapsackNode node, KnapsackNode? bestNode)
        {
            if(bestNode == null)
            {
                return true;
            }

            return node.CurrentValue > bestNode.CurrentValue + Tolerance;
        }
    }
}