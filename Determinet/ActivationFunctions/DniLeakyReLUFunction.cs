using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// Both ReLU and Leaky ReLU are activation functions used in neural networks. The main difference between them is that ReLU sets all negative
    /// values to zero while Leaky ReLU allows a small, non-zero gradient for negative input values. This helps to avoid discarding potentially
    /// important information and thus perform better than ReLU in scenarios where the data has a lot of noise or outliers1. ReLU is computationally
    /// efficient and simpler than Leaky ReLU, which makes it more suitable for shallow architectures1.
    /// </summary>
    [Serializable]
    public class DniLeakyReLUFunction : DniIActivationFunction
    {
        /// <summary>
        /// Positive value that determines the slope of the function for negative input values.
        /// </summary>
        [JsonProperty]
        public double Alpha { get; set; }

        public DniLeakyReLUFunction(DniNamedFunctionParameters? param)
        {
            if (param == null)
            {
                Alpha = 0.01;
            }
            else
            {
                Alpha = param.Get<double>("alpha", 1);
            }
        }


        public double Activation(double x)
        {
            return (0 >= x) ? Alpha * x : x;
        }

        public double Derivative(double x)
        {
            return (0 >= x) ? Alpha : 1;
        }
    }
}
