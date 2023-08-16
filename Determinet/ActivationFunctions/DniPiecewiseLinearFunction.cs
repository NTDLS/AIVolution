using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// Function that combines a linear segment for certain input range with a Leaky ReLU-like behavior for values outside that range. 
    /// </summary>
    [Serializable]
    public class DniPiecewiseLinearFunction : DniIActivationFunction
    {
        /// <summary>
        /// Linear slope value.
        /// </summary>
        [JsonProperty]
        public double Alpha { get; set; }

        /// <summary>
        /// Function output range.
        /// </summary>
        [JsonProperty]
        public DniRange Range { get; set; }

        public DniPiecewiseLinearFunction(DniNamedFunctionParameters? param)
        {

            if (param == null)
            {
                Alpha = 0.1;
                Range = new DniRange(-1, +1);
            }
            else
            {
                Alpha = param.Get<double>("alpha", 1);
                Range = param.Get<DniRange>("range", new DniRange(-1, +1));
            }
        }

        public double Activation(double x)
        {
            if (x <= Range.Min)
                return Alpha * x;
            else if (x >= Range.Max)
                return Alpha * x;
            else
                return x;
        }

        public double Derivative(double x)
        {
            if (x <= Range.Min)
                return Alpha;
            else if (x >= Range.Max)
                return Alpha;
            else
                return 1;
        }
    }
}