using System.Collections.Generic;

namespace Algorithms
{
    public class NeuralNetworkConfig
    {
        private List<NeuralNetworkLayer> layers = new List<NeuralNetworkLayer>();

        public int LayerCount => layers.Count;

        public NeuralNetworkLayer Layer(int i) => layers[i];

        public void AddLayer(LayerType type, int nodes, ActivationType activationType)
        {
            layers.Add(new NeuralNetworkLayer(type, nodes, activationType));
        }

        public void AddLayer(LayerType type, int nodes)
        {
            layers.Add(new NeuralNetworkLayer(type, nodes));
        }

        public void AddLayer(NeuralNetworkLayer layerConfig)
        {
            layers.Add(layerConfig);
        }
    }
}
