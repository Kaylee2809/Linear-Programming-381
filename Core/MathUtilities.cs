using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Programming_381.Core
{
    public static class MathUtilities
    {
        public const double Epsilon = 0.000001;
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < Epsilon;
        }
        public static bool IsPositive(double value)
        {
            return value > Epsilon;
        }
        public static bool IsNegative(double value)
        {
            return value < -Epsilon;
        }
        public static bool IsInteger(double value)
        {
            return Math.Abs(
                value - Math.Round(value))
                < Epsilon;
        }
        public static double Round(double value)
        {
            return Math.Round(value, 3);
        }
    }
}
