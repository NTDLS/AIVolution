using Determinet.ActivationFunctions;
using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;

namespace Determinet
{
    [Serializable]
    public class DNNeuralNetworkLayer
    {
        /// <summary>
        /// The number of nodes in this layer.
        /// </summary>
        public int NodeCount { get; private set; }
        public ActivationType ActivationType { get; set; }
        public object[]? Param { get; private set; }
        public string[]? Aliases { get; private set; }

        /// <summary>
        /// The type of the later (input, intermediate (hidden) or output). 
        /// </summary>
        public LayerType LayerType { get; set; }

        /// <summary>
        /// The collapse function used for activation.
        /// </summary>
        internal DNIActivationFunction ActivationFunction { get; set; }

        public DNNeuralNetworkLayer(LayerType type, int nodeCount, ActivationType activationType, string[]? nodeAlias, object[]? param)
        {
            ActivationType = activationType;
            Param = param;
            ActivationFunction = CreateActivationType(activationType, param);
            LayerType = type;
            NodeCount = nodeCount;
            Aliases = nodeAlias?.ToArray();
        }

        private DNIActivationFunction CreateActivationType(ActivationType activationType, object[]? param)
        {
            return activationType switch
            {
                ActivationType.Identity => new DNIdentityFunction(param),
                ActivationType.ReLU => new DNReLUFunction(param),
                ActivationType.Bernoulli => new DNBernoulliFunction(param),
                ActivationType.Linear => new DNLinearFunction(param),
                ActivationType.Sigmoid => new DNSigmoidFunction(param),
                ActivationType.Tanh => new DNTanhFunction(param),
                ActivationType.LeakyReLU => new DNLeakyReLUFunction(param),
                _ => throw new NotImplementedException("Unknown activation function.")
            };
        }

        public DNNeuralNetworkLayer Clone()
        {
            return new DNNeuralNetworkLayer(LayerType, NodeCount, ActivationType, Aliases, Param);
        }
    }
}
