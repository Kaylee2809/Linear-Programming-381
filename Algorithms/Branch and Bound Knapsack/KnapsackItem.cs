using Linear_Programming_381.Models;

namespace Linear_Programming_381.Algorithms.Branch_and_Bound_Knapsack
{
    public class KnapsackItem
    {
        public string VariableName {get;set;}
        public double Value{get;set;}
        public double Weight{get;set;}
        public double Ratio{get;set;}
        public int OriginalIndex{get;set;}

        public KnapsackItem(string variableName, double value, double weight, int originalIndex)
        {
            VariableName = variableName;
            Value = value;
            Weight = weight;
            OriginalIndex = originalIndex;

            Ratio = weight == 0 ? double.PositiveInfinity: value / weight;
        }
    }
}