using Determinet.Types;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Determinet
{
    [Serializable]
    public class DniNeuralNetworkLayers
    {
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

        public void Add(LayerType layerType, ActivationType activationType, int nodesCount, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(layerType, nodesCount, activationType, param, null));
        }

        public void Add(LayerType layerType, ActivationType activationType, string[] nodeAliases, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(layerType, nodeAliases.Length, activationType, param, nodeAliases));
        }

        public DniNeuralNetworkLayers Clone()
        {
            var clone = new DniNeuralNetworkLayers();
            foreach (var layer in Collection)
            {
                clone.Collection.Add(layer.Clone());
            }

            return clone;
        }
    }
}
