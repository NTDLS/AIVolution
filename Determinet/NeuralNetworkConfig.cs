using Determinet.Types;
using System.Collections.Generic;

namespace Determinet
{
    public class NeuralNetworkConfig
    {
        private readonly List<NeuralNetworkLayer> _layers = new();

        public int LayerCount => _layers.Count;

        public NeuralNetworkLayer Layer(int i) => _layers[i];

        public void AddLinearLayer(LayerType type, int nodes, double alpha, DoubleRange range)
        {
            var param = new object[2] { alpha, range };
            _layers.Add(new NeuralNetworkLayer(type, nodes, ActivationType.Linear, param));
        }

        public void AddBernoulliLayer(LayerType type, int nodes, double alpha)
        {
            var param = new object[1] { alpha };
            _layers.Add(new NeuralNetworkLayer(type, nodes, ActivationType.Bernoulli, param));
        }

        public void AddLayer(LayerType type, int nodes, ActivationType activationType)
        {
            _layers.Add(new NeuralNetworkLayer(type, nodes, activationType));
        }

        public void AddLayer(NeuralNetworkLayer layerConfig)
        {
            _layers.Add(layerConfig);
        }
    }
}
