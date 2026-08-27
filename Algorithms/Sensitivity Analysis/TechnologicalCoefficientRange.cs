namespace Linear_Programming_381.Algorithms.Sensitivity_Analysis
{
    public class TechnologicalCoefficientRange
    {
        public string VariableName { get; set; } = "";
        public int ConstraintIndex { get; set; }

        public double CurrentValue { get; set; }

        public double MinimumValue { get; set; }
        public double MaximumValue { get; set; }

        public double AllowableDecrease { get; set; }
        public double AllowableIncrease { get; set; }

        public string GetFormattedRange()
        {
            string minimum = double.IsNegativeInfinity(MinimumValue)? "-Infinity": MinimumValue.ToString("0.###");

            string maximum = double.IsPositiveInfinity(MaximumValue)? "+Infinity": MaximumValue.ToString("0.###");

            string decrease =double.IsPositiveInfinity(AllowableDecrease)? "Infinity": AllowableDecrease.ToString("0.###");

            string increase =double.IsPositiveInfinity(AllowableIncrease)? "Infinity": AllowableIncrease.ToString("0.###");

            return
                $"Constraint {ConstraintIndex + 1}, {VariableName}\r\n" +
                $"Current Coefficient: {CurrentValue:0.###}\r\n" +
                $"Minimum Value: {minimum}\r\n" +
                $"Maximum Value: {maximum}\r\n" +
                $"Allowable Decrease: {decrease}\r\n" +
                $"Allowable Increase: {increase}\r\n";
        }
    }
}
