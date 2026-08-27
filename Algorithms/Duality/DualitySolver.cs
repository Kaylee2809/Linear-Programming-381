using System;
using System.Collections.Generic;
using Linear_Programming_381.Algorithms.Primal_Simplex;
using Linear_Programming_381.Core;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.Algorithms.Duality
{
    public class DualitySolver
    {
        private const double Epsilon =
            0.000001;

        public DualityResult SolveAndVerify(
            LPModel primalModel,
            Solution primalSolution)
        {
            if (primalModel == null)
            {
                throw new ArgumentNullException(
                    nameof(primalModel));
            }

            if (primalSolution == null)
            {
                throw new ArgumentNullException(
                    nameof(primalSolution));
            }

            if (!primalSolution.IsOptimal)
            {
                throw new InvalidOperationException(
                    "The primal model must be solved " +
                    "optimally before testing duality.");
            }



            DualConverter converter =
                new DualConverter();

            LPModel dualModel =
                converter.CreateDual(
                    primalModel);



            DualModelTransformer transformer =
                new DualModelTransformer();

            DualTransformationResult transformation =
                transformer.TransformForSimplex(
                    dualModel);




            CanonicalConverter canonicalConverter =
                new CanonicalConverter();

            Tableau dualTableau =
                canonicalConverter.Convert(
                    transformation.TransformedModel);



            PrimalSimplexSolver solver =
                new PrimalSimplexSolver();

            Solution transformedDualSolution =
                solver.Solve(
                    transformation.TransformedModel,
                    dualTableau);

            if (!transformedDualSolution.IsOptimal)
            {
                throw new InvalidOperationException(
                    "The dual model could not be solved optimally.");
            }




            double primalObjective =
                primalSolution.ObjectiveValue;

            double dualObjective =
                -transformedDualSolution.ObjectiveValue;




            bool strongDuality =
                Math.Abs(
                    primalObjective -
                    dualObjective)
                <= Epsilon;



            bool weakDuality =
                primalObjective <=
                dualObjective + Epsilon;



            return new DualityResult
            {
                DualModel =
                    dualModel,


                DualSolution =
                    transformedDualSolution,

                PrimalObjectiveValue =
                    primalObjective,

                DualObjectiveValue =
                    dualObjective,

                HasStrongDuality =
                    strongDuality,

                SatisfiesWeakDuality =
                    weakDuality
            };
        }
    }
}