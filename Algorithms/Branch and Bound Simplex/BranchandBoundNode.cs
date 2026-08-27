using Linear_Programming_381.Models;

namespace Linear_Programming_381.Algorithms.Branch_and_Bound_Simplex
{
    public class BranchandBoundNode
    {
        public int NodeId {get; set;}
        public int? ParentNodeId {get;set;}
        public LPModel Model {get; set;}
        public Solution? Solution {get; set;}
        public string BranchDescription {get;set;}
        public int Depth{get;set;}
        public bool IsFathomed{get;set;}
        public string FathomReason{get;set;}
        public string BranchingVariable{get;set;}
        public double? BranchingValue{get;set;}

        public BranchandBoundNode(int nodeId, int? parentNodeId, LPModel model,
         string branchDescription, int depth)
        {
            NodeId = nodeId;
            ParentNodeId = parentNodeId;
            Model = model;
            BranchDescription = branchDescription;
            Depth = depth;

            Solution = null;

            IsFathomed = false;
            FathomReason = "";

            BranchingVariable = "";
            BranchingValue = null;
        }
    }
}