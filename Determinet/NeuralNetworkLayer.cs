using Determinet.ActivationFunctions;
using Determinet.Types;

namespace Determinet
{
    public class NeuralNetworkLayer
    {
        /// <summary>
        /// The number of nodes in this layer.
        /// </summary>
        public int NodeCount { get; private set; }
        public ActivationType ActivationType { get; private set; }
        private object[]? _param;

        public string[]? Aliases { get; private set; }

        /// <summary>
        /// The type of the later (input, intermediate (hidden) or output). 
        /// </summary>
        public LayerType LayerType { get; set; }

        /// <summary>
        /// The collapse function used for activation.
        /// </summary>
        public IActivationFunction ActivationFunction { get; set; }

        public NeuralNetworkLayer(LayerType type, int nodeCount, ActivationType activationType, string[]? nodeAlias, object[]? param)
        {
            ActivationType = activationType;
            _param = param;
            ActivationFunction = CreateActivationType(activationType, param);
            LayerType = type;
            NodeCount = nodeCount;
            Aliases = nodeAlias?.ToArray() ?? new string[nodeCount];
        }

        private IActivationFunction CreateActivationType(ActivationType activationType, object[]? param)
        {
            return activationType switch
            {
                ActivationType.Identity => new IdentityFunction(param),
                ActivationType.ReLU => new ReLUFunction(param),
                ActivationType.Bernoulli => new BernoulliFunction(param),
                ActivationType.Linear => new LinearFunction(param),
                ActivationType.Sigmoid => new SigmoidFunction(param),
                ActivationType.Tanh => new TanhFunction(param),
                ActivationType.LeakyReLU => new LeakyReLUFunction(param),
                _ => throw new NotImplementedException("Unknown activation function.")
            };
        }

        public NeuralNetworkLayer Clone()
        {
            return new NeuralNetworkLayer(LayerType, NodeCount, ActivationType, Aliases, _param);
        }
    }
}
