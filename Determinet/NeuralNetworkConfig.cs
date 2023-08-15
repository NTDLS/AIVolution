using Determinet.Types;

namespace Determinet
{
    public class NeuralNetworkConfig
    {
        private readonly List<NeuralNetworkLayer> _layers = new();

        public int LayerCount => _layers.Count;

        public NeuralNetworkLayer Layer(int i) => _layers[i];


        #region Add input layers.

        public void AddInputLayer(int nodes, ActivationType activationType)
        {
            _layers.Add(new NeuralNetworkLayer(LayerType.Input, nodes, activationType, null, null));
        }

        public void AddInputLayer(ActivationType activationType, string[] inputNodeNames)
        {
            _layers.Add(new NeuralNetworkLayer(LayerType.Input, inputNodeNames.Length, activationType, inputNodeNames, null));
        }

        public void AddLinearInputLayer(int nodes, double alpha, DoubleRange range)
        {
            var param = new object[2] { alpha, range };
            _layers.Add(new NeuralNetworkLayer(LayerType.Input, nodes, ActivationType.Linear, null, param));
        }

        public void AddBernoulliInputLayer(int nodes, double alpha)
        {
            var param = new object[1] { alpha };
            _layers.Add(new NeuralNetworkLayer(LayerType.Input, nodes, ActivationType.Bernoulli, null, param));
        }

        public void AddLinearInputLayer(string[] inputNodeNames, double alpha, DoubleRange range)
        {
            var param = new object[2] { alpha, range };
            _layers.Add(new NeuralNetworkLayer(LayerType.Input, inputNodeNames.Length, ActivationType.Linear, null, param));
        }

        public void AddBernoulliInputLayer(string[] inputNodeNames, double alpha)
        {
            var param = new object[1] { alpha };
            _layers.Add(new NeuralNetworkLayer(LayerType.Input, inputNodeNames.Length, ActivationType.Bernoulli, null, param));
        }

        #endregion

        #region Add intermediate layers.

        public void AddIntermediateLayer(int nodes, ActivationType activationType)
        {
            _layers.Add(new NeuralNetworkLayer(LayerType.Intermediate, nodes, activationType, null, null));
        }

        public void AddLinearIntermediateLayer(int nodes, double alpha, DoubleRange range)
        {
            var param = new object[2] { alpha, range };
            _layers.Add(new NeuralNetworkLayer(LayerType.Intermediate, nodes, ActivationType.Linear, null, param));
        }

        public void AddBernoulliIntermediateLayer(int nodes, double alpha)
        {
            var param = new object[1] { alpha };
            _layers.Add(new NeuralNetworkLayer(LayerType.Intermediate, nodes, ActivationType.Bernoulli, null, param));
        }

        #endregion

        #region Add output layers.

        public void AddOutputLayer(ActivationType activationType, string[] inputNodeNames)
        {
            _layers.Add(new NeuralNetworkLayer(LayerType.Output, inputNodeNames.Length, activationType, inputNodeNames, null));
        }

        public void AddOutputLayer(int nodes, ActivationType activationType)
        {
            _layers.Add(new NeuralNetworkLayer(LayerType.Output, nodes, activationType, null, null));
        }

        public void AddLinearOutputLayer(int nodes, double alpha, DoubleRange range)
        {
            var param = new object[2] { alpha, range };
            _layers.Add(new NeuralNetworkLayer(LayerType.Output, nodes, ActivationType.Linear, null, param));
        }

        public void AddBernoulliOutputLayer(int nodes, double alpha)
        {
            var param = new object[1] { alpha };
            _layers.Add(new NeuralNetworkLayer(LayerType.Output, nodes, ActivationType.Bernoulli, null, param));
        }

        public void AddLinearOutputLayer(string[] outputNodeNames, double alpha, DoubleRange range)
        {
            var param = new object[2] { alpha, range };
            _layers.Add(new NeuralNetworkLayer(LayerType.Output, outputNodeNames.Length, ActivationType.Linear, null, param));
        }

        public void AddBernoulliOutputLayer(string[] outputNodeNames, double alpha)
        {
            var param = new object[1] { alpha };
            _layers.Add(new NeuralNetworkLayer(LayerType.Output, outputNodeNames.Length, ActivationType.Bernoulli, null, param));
        }

        #endregion
    }
}
