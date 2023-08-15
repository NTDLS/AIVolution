namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// Both ReLU and Leaky ReLU are activation functions used in neural networks. The main difference between them is that ReLU sets all negative
    /// values to zero while Leaky ReLU allows a small, non-zero gradient for negative input values. This helps to avoid discarding potentially
    /// important information and thus perform better than ReLU in scenarios where the data has a lot of noise or outliers1. ReLU is computationally
    /// efficient and simpler than Leaky ReLU, which makes it more suitable for shallow architectures1.
    /// </summary>
    [Serializable]
    public class LeakyReLUFunction : IActivationFunction
    {
        public LeakyReLUFunction(object[]? param)
        {
        }

        public double Activation(double x)
        {
            return (0 >= x) ? 0.01f * x : x;
        }

        public double Derivative(double x)
        {
            return (0 >= x) ? 0.01f : 1;
        }
    }
}
