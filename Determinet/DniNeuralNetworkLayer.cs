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

        /// <summary>
        /// The type of the later (input, intermediate (hidden) or output). 
        /// </summary>
        [JsonProperty]
        public LayerType LayerType { get; set; }

        /// <summary>
        /// The collapse function used for activation.
        /// </summary>
        internal DniIFunction? Function { get; set; }

        /// <summary>
        /// Creates a new network layer.
        /// </summary>
        /// <param name="type">The type of layer (input, intermediate, or output). </param>
        /// <param name="nodeCount">The number of nodes in the layer.</param>
        /// <param name="activationType">The type of function that will be used to activate the neurons.</param>
        /// <param name="nodeAliases">The names of the nodes (optional and only used for input and output nodes).</param>
        /// <param name="param">Any optional parameters that should be passed to the activation function.</param>
        public DniNeuralNetworkLayer(DniNeuralNetworkLayers layers, LayerType layerType, int neuronCount, ActivationType activationType, DniNamedFunctionParameters? param, string[]? neuronAliases)
        {
            if (param == null)
            {
                param = new DniNamedFunctionParameters();
            }

            if (neuronAliases != null && neuronCount != neuronAliases.Length)
            {
                throw new Exception("The number of neuron aliases must match the number of specified neurons.");
            }

            if (layerType == LayerType.Input)
            {
                if (layers.Collection.Where(o => o.LayerType == LayerType.Input).Any())
                {
                    throw new Exception("Only one input layer can be added to the network.");
                }
            }
            else if (layerType == LayerType.Output)
            {
                if (layers.Collection.Where(o => o.LayerType == LayerType.Output).Any())
                {
                    throw new Exception("Only one output layer can be added to the network.");
                }
            }
            else if (layerType == LayerType.Intermediate)
            {
                if (neuronAliases != null && neuronAliases.Length > 0)
                {
                    throw new Exception("Neuron aliases are not supported on intermediate layers.");
                }
            }

            if (neuronAliases != null)
            {
                if (neuronAliases.GroupBy(o => o).Where(o => o.Count() > 1).Any())
                {
                    throw new Exception("Duplicate neuron aliases are not supported.");
                }
            }

            Layers = layers;
            ActivationType = activationType;
            Param = param;
            LayerType = layerType;
            Function = CreateActivationType(activationType, param);

            if (layerType == LayerType.Input)
            {
                if (Function != null && Function is DniIOutputFunction)
                {
                    throw new Exception("Invalid function type specified for input layer.");
                }
            }
            else if (layerType == LayerType.Intermediate)
            {
                if (Function != null && Function is DniIOutputFunction)
                {
                    throw new Exception("Invalid function type specified for intermediate layer.");
                }
            }
            else if (layerType == LayerType.Output)
            {
                if (Function != null && Function is not DniIOutputFunction)
                {
                    throw new Exception("Invalid function type specified for output layer.");
                }
            }

            Neurons = new List<DniNeuron>();

            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new DniNeuron(this, neuronAliases == null ? null : neuronAliases[i]));
            }
        }

        private DniIFunction? CreateActivationType(ActivationType activationType, DniNamedFunctionParameters param)
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

        public DniNeuralNetworkLayer Clone(DniNeuralNetworkLayers clonedLayers)
        {
            var clonedLayer = new DniNeuralNetworkLayer(clonedLayers, LayerType, Neurons.Count, ActivationType, Param, null);

            for (int i = 0; i < clonedLayer.Neurons.Count; i++)
            {
                clonedLayer.Neurons[i] = Neurons[i].Clone(clonedLayer);
            }

            return clonedLayer;
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
