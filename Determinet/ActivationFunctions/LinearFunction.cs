using Determinet.Types;
using System;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// Linear bounded activation function.
    /// </summary>
    [Serializable]
    public class LinearFunction : IActivationFunction
    {
        // linear slope value
        private double alpha;

        // function output range
        private DoubleRange range;

        public double Alpha //Linear slope value.
        {
            get { return alpha; }
            set { alpha = value; }
        }

        public DoubleRange Range //Function output range.
        {
            get { return range; }
            set { range = value; }
        }

        public LinearFunction(object[] param)
        {
            if (param == null)
            {
                alpha = 1;
                range = new DoubleRange(-1, +1);
            }
            else if (param.Length != 2)
            {
                Alpha = (double)param[0];
                Range = (DoubleRange)param[1];
            }
            else
            {
                throw new ArgumentException("Invalid number of parameters supplied for LinearFunction.");
            }
        }

        public double Activation(double x)
        {
            double y = alpha * x;

            if (y > range.Max)
                return range.Max;
            else if (y < range.Min)
                return range.Min;
            return y;
        }

        public double Derivative(double x)
        {
            double y = alpha * x;

            if (y <= range.Min || y >= range.Max)
                return 0;
            return alpha;
        }
    }
}
