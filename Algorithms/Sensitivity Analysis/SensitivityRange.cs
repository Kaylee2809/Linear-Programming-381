namespace Linear_Programming_381.Algorithms.Sensitivity_Analysis
{
    public class SensitivityRange
    {
        public string ItemName{get;set;}

        public double CurrentValue{get;set;}
        public double MinimumValue{get;set;}
        public double MaximumValue{get;set;}
        public double AllowableDecrease{get;set;}
        public double AllowableIncrease{get;set;}
        public string Description{get;set;} = "";

        public string GetFormattedRange()
        {
            string minimum = double.IsNegativeInfinity(MinimumValue) ? "-Infinity": MinimumValue.ToString("0.###");

            string maximum = double.IsPositiveInfinity(MaximumValue) ? "+Infinity": MaximumValue.ToString("0.###");

            string decrease = double.IsPositiveInfinity(AllowableDecrease) ? "Infinity" : AllowableDecrease.ToString("0.###");

            string increase = double.IsPositiveInfinity(AllowableIncrease)? "Infinity" : AllowableIncrease.ToString("0.###");

            return $"{ItemName}\r\nCurrent Value: {CurrentValue:0.###}\r\nMinimum Value: {minimum}\r\nMaximum Value: {maximum}\r\n" +
            $"Allowable Decrease: {decrease}\r\nAllowable Increase: {increase}\r\n";
        }
    }
}