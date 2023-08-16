﻿using Determinet.ActivationFunctions;
using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet
{

    public class DniNeuron
    {
        private DniNeuralNetworkLayer Layer { get; set; }

        [JsonProperty]
        public double Value { get; set; }

        public DniNeuron(DniNeuralNetworkLayer layer)
        {
            Layer = layer;
            Value = 0;
        }
    }

    [Serializable]
    public class DniNeuralNetworkLayer
    {
        /// <summary>
        /// The number of nodes in this layer.
        /// </summary>
        [JsonProperty]
        public List<DniNeuron> Neurons { get; set; }

        [JsonProperty]
        public ActivationType ActivationType { get; set; }

        [JsonProperty]
        public DniNamedFunctionParameters? Param { get; private set; }

        [JsonProperty]
        public string[]? Aliases { get; private set; }

        /// <summary>
        /// The type of the later (input, intermediate (hidden) or output). 
        /// </summary>
        [JsonProperty]
        public LayerType LayerType { get; set; }

        /// <summary>
        /// The collapse function used for activation.
        /// </summary>
        internal DniIActivationFunction ActivationFunction { get; set; }

        /// <summary>
        /// Creates a new network layer.
        /// </summary>
        /// <param name="type">The type of layer (input, intermediate, or output). </param>
        /// <param name="nodeCount">The number of nodes in the layer.</param>
        /// <param name="activationType">The type of function that will be used to activate the neurons.</param>
        /// <param name="nodeAliases">The names of the nodes (optional and only used for input and output nodes).</param>
        /// <param name="param">Any optional parameters that should be passed to the activation function.</param>
        public DniNeuralNetworkLayer(LayerType type, int neuronCount, ActivationType activationType, DniNamedFunctionParameters? param, string[]? nodeAliases)
        {
            ActivationType = activationType;
            Param = param;
            ActivationFunction = CreateActivationType(activationType, param);
            LayerType = type;
            Aliases = nodeAliases?.ToArray();

            Neurons = new List<DniNeuron>();

            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new DniNeuron(this));
            }
        }

        private DniIActivationFunction CreateActivationType(ActivationType activationType, DniNamedFunctionParameters? param)
        {
            return activationType switch
            {
                ActivationType.Identity => new DniIdentityFunction(param),
                ActivationType.ReLU => new DniReLUFunction(param),
                ActivationType.Bernoulli => new DniBernoulliFunction(param),
                ActivationType.Linear => new DniLinearFunction(param),
                ActivationType.Sigmoid => new DniSigmoidFunction(param),
                ActivationType.Tanh => new DniTanhFunction(param),
                ActivationType.LeakyReLU => new DniLeakyReLUFunction(param),
                _ => throw new NotImplementedException("Unknown activation function.")
            };
        }

        public DniNeuralNetworkLayer Clone()
        {
            return new DniNeuralNetworkLayer(LayerType, Neurons.Count, ActivationType, Param, Aliases);
        }
    }
}