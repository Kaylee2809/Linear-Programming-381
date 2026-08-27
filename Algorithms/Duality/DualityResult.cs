using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.Algorithms.Duality
{
    public class DualityResult
    {
        public LPModel DualModel{get;set;} = null!;

        public Solution DualSolution{get;set;} = null!;

        public double PrimalObjectiveValue{get;set;}

        public double DualObjectiveValue{get;set;}

        public bool HasStrongDuality{get;set;}

        public bool SatisfiesWeakDuality{get;set;}
    }
}
