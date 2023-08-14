using Determinet.ActivationFunctions;
using Determinet.Types;
using System;

namespace Determinet
{
    public class NeuralNetworkLayer
    {
        /// <summary>
        /// The number of nodes in this layer.
        /// </summary>
        public int Nodes { get; private set; }

        public string[] Aliases { get; private set; }

        /// <summary>
        /// The type of the later (input, intermediate (Hidden) or output). 
        /// </summary>
        public LayerType LayerType { get; set; }

        /// <summary>
        /// The collapse function used for activation.
        /// </summary>
        public IActivationFunction ActivationFunction { get; set; }

        public NeuralNetworkLayer(LayerType type, int nodeCount, ActivationType activationType, string []nodeAlias = null, object[] param = null)
        {
            ActivationFunction = CreateActivationType(activationType, param);
            LayerType = type;
            Nodes = nodeCount;
            Aliases = nodeAlias;
            if (Aliases == null)
            {
                Aliases = new string[nodeCount];
            }
        }

        private IActivationFunction CreateActivationType(ActivationType activationType, object[] param)
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
    }
}
