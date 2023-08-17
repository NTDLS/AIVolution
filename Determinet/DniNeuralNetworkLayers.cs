using Determinet.Types;
using Newtonsoft.Json;
using System.Collections;

namespace Determinet
{
    [Serializable]
    public class DniNeuralNetworkLayers //: IEnumerable<DniNeuralNetworkLayer>
    {
        [JsonIgnore]
        public DniNeuralNetwork Network { get; internal set; }

        [JsonProperty]
        internal List<DniNeuralNetworkLayer> Collection { get; private set; } = new();

        [JsonProperty]
        internal int Count => Collection.Count;

        #region IEnumerable.

        [JsonIgnore]
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

        /*
        public IEnumerator<DniNeuralNetworkLayer> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        */

        #endregion

        public DniNeuralNetworkLayers(DniNeuralNetwork network)
        {
            Network = network;
        }

        public void AddInput(ActivationType activationType, int nodesCount, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network.Layers, LayerType.Input, nodesCount, activationType, param, null));
        }

        public void AddInput(ActivationType activationType, string[] nodeAliases, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network.Layers, LayerType.Input, nodeAliases.Length, activationType, param, nodeAliases));
        }

        public void AddIntermediate(ActivationType activationType, int nodesCount, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network.Layers, LayerType.Intermediate, nodesCount, activationType, param, null));
        }

        public void AddOutput(int nodesCount, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network.Layers, LayerType.Output, nodesCount, ActivationType.None, param, null));
        }

        public void AddOutput(string[] nodeAliases, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network.Layers, LayerType.Output, nodeAliases.Length, ActivationType.None, param, nodeAliases));
        }

        public void AddOutput(ActivationType activationType, int nodesCount, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network.Layers, LayerType.Output, nodesCount, activationType, param, null));
        }

        public void AddOutput(ActivationType activationType, string[] nodeAliases, DniNamedFunctionParameters? param = null)
        {
            Collection.Add(new DniNeuralNetworkLayer(Network.Layers, LayerType.Output, nodeAliases.Length, activationType, param, nodeAliases));
        }

        #region Genetic.

        public DniNeuralNetworkLayers Clone()
        {
            var clone = new DniNeuralNetworkLayers(Network);
            foreach (var layer in Collection)
            {
                clone.Collection.Add(layer.Clone());
            }

            return clone;
        }

        public void Mutate(double mutationProbability, double mutationSeverity)
        {
            foreach (var layer in Collection)
            {
                layer.Mutate(mutationProbability, mutationSeverity);
            }
        }

        #endregion
    }
}
