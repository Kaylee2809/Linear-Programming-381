using System.Collections.Generic;

namespace Linear_Programming_381.Algorithms.Branch_and_Bound_Knapsack
{
    public class KnapsackNode
    {
        public int NodeId{get;set;}
        public int? ParentNodeId{get;set;}
        public int Depth{get;set;}
        public int Level{get;set;}
        public double CurrentValue{get;set;}
        public double CurrentWeight{get;set;}
        public double Bound{get;set;}
        public List<int> Decisions{get;set;}
        public bool IsFathomed{get;set;}
        public string FathomReason{get;set;}
        public string BranchDescription{get;set;}

        public KnapsackNode(int nodeId, int? parentNodeId, int depth, int level, double currentValue, double currentWeight,
        List<int> decisions, string branchDescription)
        {
            NodeId = nodeId;
            ParentNodeId = parentNodeId;
            Depth = depth;
            Level = level;
            CurrentValue = currentValue;
            CurrentWeight = currentWeight;
            BranchDescription = branchDescription;
            
            Decisions = new List<int>(decisions);

            Bound = 0;

            IsFathomed = false;
            FathomReason = "";
        }
    }
}