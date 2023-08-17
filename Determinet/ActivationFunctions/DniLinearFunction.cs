using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// Linear bounded activation function.
    /// </summary>
    [Serializable]
    public class DniLinearFunction : DniIActivationFunction
    {
        // linear slope value
        private double alpha;

        // function output range
        private DniRange range;

        [JsonProperty]
        public double Alpha //Linear slope value.
        {
            get { return alpha; }
            set { alpha = value; }
        }

        [JsonProperty]
        public DniRange Range //Function output range.
        {
            get { return range; }
            set { range = value; }
        }

        public DniLinearFunction(DniNamedFunctionParameters param)
        {
            Alpha = param.Get<double>("Alpha", 1);
            Range = param.Get<DniRange>("Range", new DniRange(-1, +1));
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
