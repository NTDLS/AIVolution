using Determinet.Types;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Determinet
{
    [Serializable]
    public class DniNeuralNetworkLayers
    {
        public DniNeuralNetwork Network { get; private set; }

        [JsonProperty]
        internal List<DniNeuralNetworkLayer> Collection { get; private set; } = new();

        [JsonProperty]
        internal int Count => Collection.Count;

        public DniNeuralNetworkLayer Get(int i) => Collection[i];

        public DniNeuralNetworkLayer this[int index]
        {
            get
            {
                if (index < 0 || index >= Collection.Count)
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }
                return Collection[index];
            }
        }

        public DniNeuralNetworkLayers(DniNeuralNetwork network)
        {
            Network = network;
        }

        public void Add(LayerType layerType, ActivationType activationType, int nodesCount, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network, layerType, nodesCount, activationType, param, null));
        }

        public void Add(LayerType layerType, ActivationType activationType, string[] nodeAliases, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network, layerType, nodeAliases.Length, activationType, param, nodeAliases));
        }

        public DniNeuralNetworkLayers Clone()
        {
            var clone = new DniNeuralNetworkLayers(Network);
            foreach (var layer in Collection)
            {
                clone.Collection.Add(layer.Clone());
            }

            return clone;
        }
    }
}
