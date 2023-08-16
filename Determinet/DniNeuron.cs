using Newtonsoft.Json;

namespace Determinet
{
    [Serializable]
    public class DniNeuron
    {
        public DniNeuralNetworkLayer Layer { get; private set; }

        [JsonProperty]
        public double Bias { get; set; }

        [JsonProperty]
        public double Value { get; set; }

        [JsonProperty]
        public double[] Weights { get; set; }

        public DniNeuron(DniNeuralNetworkLayer layer)
        {
            Layer = layer;
            Value = 0;
            Bias = DniUtility.GetRandomBiasValue();

            if (layer.Network.Layers.Count > 0)
            {
                var previousLayer = layer.Network.Layers[layer.Network.Layers.Count - 1];

                /// Initializes random array for the weights being held in the network.
                Weights = new double[previousLayer.Neurons.Count];

                for (int i = 1; i < Weights.Length; i++)
                {
                    Weights[i] = DniUtility.GetRandomWeightValue();
                }
            }
            else Weights = new double[0];
        }

        public DniNeuron Clone()
        {
            return new DniNeuron(Layer)
            {
                Bias = Bias,
                Layer = Layer,
                Value = Value,
                Weights = Weights.ToArray()
            };
        }

        /// <summary>
        /// Mutation for genetic implementations.
        /// </summary>
        public void Mutate(double mutationProbability, double mutationSeverity)
        {
            Bias = DniUtility.FlipCoin(mutationProbability) ? Bias += DniUtility.NextDouble(-mutationSeverity, mutationSeverity) : Bias;

            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = DniUtility.FlipCoin(mutationProbability) ? Weights[i] += DniUtility.NextDouble(-mutationSeverity, mutationSeverity) : Weights[i];
            }
        }
    }
}
