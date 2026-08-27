using Linear_Programming_381.Models;
using System.Collections.Generic;

namespace Linear_Programming_381.Algorithms.Branch_and_Bound_Knapsack
{
    public class BranchAndBoundKnapsackResult : Solution
    {
        public List<KnapsackNode> Nodes {get;set;}
        public int? BestNodeId {get;set;}

        public string SearchTree {get;set;}

        public BranchAndBoundKnapsackResult()
        {
            Nodes = new List<KnapsackNode>();
            BestNodeId = null;
            SearchTree = "";
        }
    }
}