using Linear_Programming_381.Models;
using System.Collections.Generic;

namespace Linear_Programming_381.Algorithms.Branch_and_Bound_Simplex
{
    public class BranchandBoundResult : Solution
    {
        public List<BranchandBoundNode> Nodes {get;set;}

        public int? BestNodeId{get;set;}

        public string SearchTree{get;set;}

        public BranchandBoundResult()
        {
            Nodes = new List<BranchandBoundNode>();
            BestNodeId = null;
            SearchTree = "";
        }
    }
}