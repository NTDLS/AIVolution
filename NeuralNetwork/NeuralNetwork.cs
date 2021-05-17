using System;
using System.Collections.Generic;
using System.IO;

namespace Algorithms
{
    public class NeuralNetwork
    {
        //Fundamental 
        private int[] layers;//layers
        private float[][] neurons;//neurons
        private float[][] biases;//biasses
        private float[][][] weights;//weights

        //Genetic
        public float Fitness = 0;//fitness

        //Backprop
        public float learningRate = 0.01f;//learning rate
        public float cost = 0; //Not used in calculions. Used to identify the performance of the network.

        //Other
        private Random random;
        private NeuralNetworkConfig configuration;
        private int randomSeed = 0;

        public void Reseed(int randomSeed = 0)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            if (randomSeed == 0)
            {
                this.randomSeed = Guid.NewGuid().GetHashCode();
            }
            else
            {
                this.randomSeed = randomSeed;
            }

            random = new Random(this.randomSeed);
        }

        public NeuralNetwork(NeuralNetworkConfig configuration, float learningRate, int randomSeed = 0)
        {
            if (randomSeed == 0)
            {
                this.randomSeed = Guid.NewGuid().GetHashCode();
            }
            else
            {
                this.randomSeed = randomSeed;
            }

            random = new Random(this.randomSeed);

            this.configuration = configuration;
            this.learningRate = learningRate;

            this.layers = new int[configuration.LayerCount];

            for (int i = 0; i < configuration.LayerCount; i++)
            {
                this.layers[i] = configuration.Layer(i).Nodes;
            }

            InitNeurons();
            InitBiases();
            InitWeights();
        }

        /// <summary>
        /// Feed forward, inputs >==> outputs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public float[] FeedForward(float[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                neurons[0][i] = inputs[i];
            }
            for (int i = 1; i < layers.Length; i++)
            {
                int layer = i - 1;
                for (int j = 0; j < layers[i]; j++)
                {
                    float value = 0f;
                    for (int k = 0; k < layers[i - 1]; k++)
                    {
                        value += weights[i - 1][j][k] * neurons[i - 1][k];
                    }
                    neurons[i][j] = activate(value + biases[i - 1][j], layer);
                }
            }
            return neurons[layers.Length - 1];
        }

        #region Initilization.

        /// <summary>
        /// Create empty storage array for the neurons in the network.
        /// </summary>
        private void InitNeurons()
        {
            List<float[]> neuronsList = new List<float[]>();
            for (int i = 0; i < layers.Length; i++)
            {
                neuronsList.Add(new float[layers[i]]);
            }
            neurons = neuronsList.ToArray();
        }

        /// <summary>
        /// Initializes random array for the biases being held within the network.
        /// </summary>
        private void InitBiases()
        {
            List<float[]> biasList = new List<float[]>();
            for (int i = 1; i < layers.Length; i++)
            {
                float[] bias = new float[layers[i]];
                for (int j = 0; j < layers[i]; j++)
                {
                    bias[j] = GetRandomBias();
                }
                biasList.Add(bias);
            }
            biases = biasList.ToArray();
        }

        /// <summary>
        /// Initializes random array for the weights being held in the network.
        /// </summary>
        private void InitWeights()
        {
            List<float[][]> weightsList = new List<float[][]>();
            for (int i = 1; i < layers.Length; i++)
            {
                List<float[]> layerWeightsList = new List<float[]>();
                int neuronsInPreviousLayer = layers[i - 1];
                for (int j = 0; j < layers[i]; j++)
                {
                    float[] neuronWeights = new float[neuronsInPreviousLayer];
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        neuronWeights[k] = GetRandomBias();
                    }
                    layerWeightsList.Add(neuronWeights);
                }
                weightsList.Add(layerWeightsList.ToArray());
            }
            weights = weightsList.ToArray();
        }

        #endregion

        #region Activation functions.

        /// <summary>
        /// Activation functions
        /// </summary>
        /// <param name="value"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public float activate(float value, int layer)
        {
            switch (configuration.Layer(layer).ActivationType)
            {
                case ActivationType.Sigmoid:
                    return sigmoid(value);
                case ActivationType.Tanh:
                    return tanh(value);
                case ActivationType.Relu:
                    return relu(value);
                case ActivationType.LeakyRelu:
                    return leakyrelu(value);
                default:
                    return relu(value);
            }
        }

        /// <summary>
        /// Activation function derivatives
        /// </summary>
        /// <param name="value"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public float activateDer(float value, int layer)
        {
            switch (configuration.Layer(layer).ActivationType)
            {
                case ActivationType.Sigmoid:
                    return sigmoidDer(value);
                case ActivationType.Tanh:
                    return tanhDer(value);
                case ActivationType.Relu:
                    return reluDer(value);
                case ActivationType.LeakyRelu:
                    return leakyreluDer(value);
                default:
                    return reluDer(value);
            }
        }

        /// <summary>
        /// Activation functions and their corrosponding derivatives
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float sigmoid(float x)
        {
            float k = (float)Math.Exp(x);
            return k / (1.0f + k);
        }
        public float tanh(float x)
        {
            return (float)Math.Tanh(x);
        }
        public float relu(float x)
        {
            return (0 >= x) ? 0 : x;
        }
        public float leakyrelu(float x)
        {
            return (0 >= x) ? 0.01f * x : x;
        }
        public float sigmoidDer(float x)
        {
            return x * (1 - x);
        }
        public float tanhDer(float x)
        {
            return 1 - (x * x);
        }
        public float reluDer(float x)
        {
            return (0 >= x) ? 0 : 1;
        }
        public float leakyreluDer(float x)
        {
            return (0 >= x) ? 0.01f : 1;
        }

        #endregion

        #region Backpropagation.

        public void BackPropagate(float[] inputs, float[] expected)//backpropogation;
        {
            float[] output = FeedForward(inputs);//runs feed forward to ensure neurons are populated correctly

            cost = 0;
            for (int i = 0; i < output.Length; i++)
            {
                cost += (float)Math.Pow(output[i] - expected[i], 2);//calculated cost of network
            }
            cost = cost / 2;

            float[][] gamma;

            List<float[]> gammaList = new List<float[]>();
            for (int i = 0; i < layers.Length; i++)
            {
                gammaList.Add(new float[layers[i]]);
            }
            gamma = gammaList.ToArray();//gamma initialization

            int layer = layers.Length - 2;
            for (int i = 0; i < output.Length; i++) gamma[layers.Length - 1][i] = (output[i] - expected[i]) * activateDer(output[i], layer);//Gamma calculation
            for (int i = 0; i < layers[layers.Length - 1]; i++)//calculates the w' and b' for the last layer in the network
            {
                biases[layers.Length - 2][i] -= gamma[layers.Length - 1][i] * learningRate;
                for (int j = 0; j < layers[layers.Length - 2]; j++)
                {

                    weights[layers.Length - 2][i][j] -= gamma[layers.Length - 1][i] * neurons[layers.Length - 2][j] * learningRate;//*learning 
                }
            }

            for (int i = layers.Length - 2; i > 0; i--)//runs on all hidden layers
            {
                layer = i - 1;
                for (int j = 0; j < layers[i]; j++)//outputs
                {
                    gamma[i][j] = 0;
                    for (int k = 0; k < gamma[i + 1].Length; k++)
                    {
                        gamma[i][j] += gamma[i + 1][k] * weights[i][k][j];
                    }
                    gamma[i][j] *= activateDer(neurons[i][j], layer);//calculate gamma
                }
                for (int j = 0; j < layers[i]; j++)//itterate over outputs of layer
                {
                    biases[i - 1][j] -= gamma[i][j] * learningRate;//modify biases of network
                    for (int k = 0; k < layers[i - 1]; k++)//itterate over inputs to layer
                    {
                        weights[i - 1][j][k] -= gamma[i][j] * neurons[i - 1][k] * learningRate;//modify weights of network
                    }
                }
            }
        }

        #endregion

        #region Genetic implementation.

        /// <summary>
        /// Used as a simple mutation function for any genetic implementations.
        /// </summary>
        public void Mutate(double mutationProbability, float mutationSeverity, int randomSeed = 0)
        {
            Reseed(randomSeed);

            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = FlipCoin(mutationProbability) ? biases[i][j] += NextFloat(-mutationSeverity, mutationSeverity) : biases[i][j];
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = FlipCoin(mutationProbability) ? weights[i][j][k] += NextFloat(-mutationSeverity, mutationSeverity) : weights[i][j][k];
                    }
                }
            }
        }

        /// <summary>
        /// Comparing For genetic implementations. Used for sorting based on the fitness of the network
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(NeuralNetwork other)
        {
            if (other == null) return 1;

            if (Fitness > other.Fitness)
                return 1;
            else if (Fitness < other.Fitness)
                return -1;
            else
                return 0;
        }

        /// <summary>
        /// For creating a deep copy, to ensure arrays are serialzed.
        /// </summary>
        /// <param name="nn"></param>
        /// <returns></returns>
        public NeuralNetwork Clone()
        {
            var nn = new NeuralNetwork(this.configuration, this.learningRate, this.randomSeed);

            nn.Fitness = this.Fitness;

            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    nn.biases[i][j] = biases[i][j];
                }
            }
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        nn.weights[i][j][k] = weights[i][j][k];
                    }
                }
            }
            return nn;
        }

        #endregion

        #region Load / Save.

        /// <summary>
        /// Loads the biases and weights from within a file into the neural network.
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            TextReader tr = new StreamReader(path);
            int NumberOfLines = (int)new FileInfo(path).Length;
            string[] ListLines = new string[NumberOfLines];
            int index = 1;
            for (int i = 1; i < NumberOfLines; i++)
            {
                ListLines[i] = tr.ReadLine();
            }
            tr.Close();
            if (new FileInfo(path).Length > 0)
            {
                for (int i = 0; i < biases.Length; i++)
                {
                    for (int j = 0; j < biases[i].Length; j++)
                    {
                        biases[i][j] = float.Parse(ListLines[index]);
                        index++;
                    }
                }

                for (int i = 0; i < weights.Length; i++)
                {
                    for (int j = 0; j < weights[i].Length; j++)
                    {
                        for (int k = 0; k < weights[i][j].Length; k++)
                        {
                            weights[i][j][k] = float.Parse(ListLines[index]); ;
                            index++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save the biases and weights within the network to a file.
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            File.Create(path).Close();
            StreamWriter writer = new StreamWriter(path, true);

            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    writer.WriteLine(biases[i][j]);
                }
            }

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        writer.WriteLine(weights[i][j][k]);
                    }
                }
            }
            writer.Close();
        }

        #endregion

        #region Random implementation.

        /// <summary>
        /// Flips a coin with a probability between 0.0 - 1.0.
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        private bool FlipCoin(double probability)
        {
            double d = random.Next(0, 1000);

            bool result = (d / 1000 >= probability);

            return result;
        }

        private bool FlipCoin()
        {
            return random.Next(0, 100) >= 50;
        }

        private float GetRandomBias()
        {
            if (FlipCoin())
            {
                return (float)(random.NextDouble() / 0.5);
            }
            return (float)((random.NextDouble() / 0.5f) * -1);
        }

        private double NextDouble(double minimum, double maximum)
        {
            if (minimum < 0)
            {
                minimum = Math.Abs(minimum);

                if (FlipCoin())
                {
                    return (random.NextDouble() * (maximum - minimum) + minimum) * -1;
                }
            }
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        private float NextFloat(float minimum, float maximum)
        {
            return (float)NextDouble(minimum, maximum);
        }

        #endregion
    }
}
