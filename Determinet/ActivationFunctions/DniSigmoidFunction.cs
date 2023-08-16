using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// The sigmoid activation function, also called the logistic function, is traditionally a very popular activation function for neural networks.
    /// The input to the function is transformed into a value between 0.0 and 1.0. Inputs that are much larger than 1.0 are transformed to the value 1.0,
    /// similarly, values much smaller than 0.0 are snapped to 0.0. The shape of the function for all possible inputs is an S-shape from zero up
    /// through 0.5 to 1.0. For a long time, through the early 1990s, it was the default activation used on neural networks.
    /// </summary>
    [Serializable]
    public class DniSigmoidFunction : DniIActivationFunction
    {
        public DniSigmoidFunction(DniNamedFunctionParameters? param)
        {
        }

        public double Activation(double x)
        {
            double k = (double)Math.Exp(x);
            return k / (1.0f + k);
        }

        public double Derivative(double x)
        {
            return x * (1 - x);
        }
    }
}
