using System;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// An identity function is a function that returns the same value as its input. In machine learning, an identity
    /// function is often used as a default activation function for a layer of a neural network. When used as an activation
    /// function, an identity function simply passes the input through the layer unchanged.
    /// </summary>
    [Serializable]
    public class IdentityFunction : IActivationFunction
    {
        public IdentityFunction(object[] param)
        {
        }

        public double Activation(double x) => x;
        public double Derivative(double x) => 1;
    }
}
