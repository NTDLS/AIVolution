using Determinet.ActivationFunctions;
using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet
{
    [Serializable]
    public class DniNeuralNetworkLayer
    {
        [JsonIgnore]
        public DniNeuralNetworkLayers Layers { get; internal set; }

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
        internal DniIActivationFunction? ActivationFunction { get; set; }

        /// <summary>
        /// Creates a new network layer.
        /// </summary>
        /// <param name="type">The type of layer (input, intermediate, or output). </param>
        /// <param name="nodeCount">The number of nodes in the layer.</param>
        /// <param name="activationType">The type of function that will be used to activate the neurons.</param>
        /// <param name="nodeAliases">The names of the nodes (optional and only used for input and output nodes).</param>
        /// <param name="param">Any optional parameters that should be passed to the activation function.</param>
        public DniNeuralNetworkLayer(DniNeuralNetworkLayers layers, LayerType type, int neuronCount, ActivationType activationType, DniNamedFunctionParameters? param, string[]? nodeAliases)
        {
            Layers = layers;
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

        private DniIActivationFunction? CreateActivationType(ActivationType activationType, DniNamedFunctionParameters? param)
        {
            return activationType switch
            {
                ActivationType.None => null,
                ActivationType.Identity => new DniIdentityFunction(param),
                ActivationType.ReLU => new DniReLUFunction(param),
                ActivationType.BinaryChaos => new DniBinaryChaosFunction(param),
                ActivationType.Linear => new DniLinearFunction(param),
                ActivationType.Sigmoid => new DniSigmoidFunction(param),
                ActivationType.Tanh => new DniTanhFunction(param),
                ActivationType.LeakyReLU => new DniLeakyReLUFunction(param),
                _ => throw new NotImplementedException("Unknown activation function.")
            };
        }

        public DniNeuralNetworkLayer Clone()
        {
            var clone = new DniNeuralNetworkLayer(Layers, LayerType, Neurons.Count, ActivationType, Param, Aliases);

            for (int i = 0; i < clone.Neurons.Count; i++)
            {
                clone.Neurons[i] = Neurons[i].Clone();
            }

            return clone;
        }

        /// <summary>
        /// Mutation for genetic implementations.
        /// </summary>
        public void Mutate(double mutationProbability, double mutationSeverity)
        {
            foreach (var neuron in Neurons)
            {
                neuron.Mutate(mutationProbability, mutationSeverity);
            }
        }
    }
}
