namespace Algorithms
{
    public class NeuralNetworkLayer
    {
        /// <summary>
        /// The number of nodes in this layer.
        /// </summary>
        public int Nodes { get; set; }

        /// <summary>
        /// The type of the later (input, intermediate (Hidden) or output). 
        /// </summary>
        public LayerType LayerType { get; set; }

        /// <summary>
        /// The collapse function used for activation.
        /// </summary>
        public ActivationType ActivationType { get; set; }

        public NeuralNetworkLayer(LayerType type, int nodes, ActivationType activationType)
        {
            this.ActivationType = ActivationType;
            this.LayerType = type;
            this.Nodes = nodes;
        }

        public NeuralNetworkLayer(LayerType type, int nodes)
        {
            this.LayerType = type;
            this.Nodes = nodes;
        }

    }
}
