using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet
{
    [Serializable]
    public class DNNeuralNetworkLayers
    {
        [JsonProperty]
        internal List<DNNeuralNetworkLayer> Collection { get; private set; } = new();

        [JsonProperty]
        internal int Count => Collection.Count;

        public DNNeuralNetworkLayer Get(int i) => Collection[i];

        public DNNeuralNetworkLayer this[int index]
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

        #region Add input layers.

        public void AddInputLayer(int nodes, ActivationType activationType)
        {
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Input, nodes, activationType, null, null));
        }

        public void AddInputLayer(ActivationType activationType, string[] inputNodeNames)
        {
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Input, inputNodeNames.Length, activationType, inputNodeNames, null));
        }

        public void AddLinearInputLayer(int nodes, double alpha, DNRangeD range)
        {
            var param = new object[2] { alpha, range };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Input, nodes, ActivationType.Linear, null, param));
        }

        public void AddBernoulliInputLayer(int nodes, double alpha)
        {
            var param = new object[1] { alpha };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Input, nodes, ActivationType.Bernoulli, null, param));
        }

        public void AddLinearInputLayer(string[] inputNodeNames, double alpha, DNRangeD range)
        {
            var param = new object[2] { alpha, range };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Input, inputNodeNames.Length, ActivationType.Linear, null, param));
        }

        public void AddBernoulliInputLayer(string[] inputNodeNames, double alpha)
        {
            var param = new object[1] { alpha };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Input, inputNodeNames.Length, ActivationType.Bernoulli, null, param));
        }

        #endregion

        #region Add intermediate layers.

        public void AddIntermediateLayer(int nodes, ActivationType activationType)
        {
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Intermediate, nodes, activationType, null, null));
        }

        public void AddLinearIntermediateLayer(int nodes, double alpha, DNRangeD range)
        {
            var param = new object[2] { alpha, range };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Intermediate, nodes, ActivationType.Linear, null, param));
        }

        public void AddBernoulliIntermediateLayer(int nodes, double alpha)
        {
            var param = new object[1] { alpha };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Intermediate, nodes, ActivationType.Bernoulli, null, param));
        }

        #endregion

        #region Add output layers.

        public void AddOutputLayer(ActivationType activationType, string[] inputNodeNames)
        {
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Output, inputNodeNames.Length, activationType, inputNodeNames, null));
        }

        public void AddOutputLayer(int nodes, ActivationType activationType)
        {
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Output, nodes, activationType, null, null));
        }

        public void AddLinearOutputLayer(int nodes, double alpha, DNRangeD range)
        {
            var param = new object[2] { alpha, range };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Output, nodes, ActivationType.Linear, null, param));
        }

        public void AddBernoulliOutputLayer(int nodes, double alpha)
        {
            var param = new object[1] { alpha };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Output, nodes, ActivationType.Bernoulli, null, param));
        }

        public void AddLinearOutputLayer(string[] outputNodeNames, double alpha, DNRangeD range)
        {
            var param = new object[2] { alpha, range };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Output, outputNodeNames.Length, ActivationType.Linear, null, param));
        }

        public void AddBernoulliOutputLayer(string[] outputNodeNames, double alpha)
        {
            var param = new object[1] { alpha };
            Collection.Add(new DNNeuralNetworkLayer(LayerType.Output, outputNodeNames.Length, ActivationType.Bernoulli, null, param));
        }

        #endregion

        public DNNeuralNetworkLayers Clone()
        {
            var clone = new DNNeuralNetworkLayers();
            foreach (var layer in Collection)
            {
                clone.Collection.Add(layer.Clone());
            }

            return clone;
        }
    }
}
