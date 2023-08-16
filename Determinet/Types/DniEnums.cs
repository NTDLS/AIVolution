namespace Determinet.Types
{
    [Serializable]
    public enum LayerType
    {
        Input,
        Intermediate,
        Output
    }

    [Serializable]
    public enum ActivationType
    {
        /// <summary>
        /// Default simple passthrough activation function.
        /// </summary>
        Identity,
        Bernoulli,
        Linear,
        /// <summary>
        /// Rectified linear unit activation function.
        /// </summary>
        ReLU,
        Sigmoid,
        /// <summary>
        /// Tanh (logistic function) activation function.
        /// </summary>
        Tanh,
        LeakyReLU //Leaky-Rectified linear unit
    }
}
