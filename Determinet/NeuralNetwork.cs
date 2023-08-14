using Determinet.ActivationFunctions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Determinet
{
    public class NeuralNetwork
    {
        //Fundamental.
        private readonly int[] _layers;
        private double[][] _neurons;
        private double[][] _biases;
        private double[][][] _weights;

        //Genetic.
        public double Fitness { get; set; } = 0;

        //Backprop.
        public double LearningRate { get; set; } = 0.01f;
        public double Cost { get; private set; } = 0; //Not used in calculions, only to identify the performance of the network.

        //Other.
        private Random _random;
        private readonly NeuralNetworkConfig _configuration;
        private int _randomSeed = 0;

        public void Reseed(int randomSeed = 0)
        {
            if (randomSeed == 0)
            {
                _randomSeed = Guid.NewGuid().GetHashCode();
            }
            else
            {
                _randomSeed = randomSeed;
            }

            _random = new Random(randomSeed);
        }

        public NeuralNetwork(NeuralNetworkConfig configuration, double learningRate, int randomSeed = 0)
        {
            Reseed(randomSeed);

            _configuration = configuration;
            LearningRate = learningRate;

            _layers = new int[configuration.LayerCount];

            for (int i = 0; i < configuration.LayerCount; i++)
            {
                _layers[i] = configuration.Layer(i).Nodes;
            }

            InitializeNeurons();
            InitializeBiases();
            InitializeWeights();
        }

        /// <summary>
        /// Feed forward, inputs >==> outputs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] FeedForward(double[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                _neurons[0][i] = inputs[i];
            }

            for (int i = 1; i < _layers.Length; i++)
            {
                int layer = i - 1;
                for (int j = 0; j < _layers[i]; j++)
                {
                    double value = 0;
                    for (int k = 0; k < _layers[i - 1]; k++)
                    {
                        value += _weights[i - 1][j][k] * _neurons[i - 1][k];
                    }

                    _neurons[i][j] = _configuration.Layer(layer).ActivationFunction.Activation(value + _biases[i - 1][j]);

                    var mahcine = _configuration.Layer(layer).ActivationFunction as IActivationMachine;
                    if (mahcine != null)
                    {
                        _neurons[i][j] = mahcine.Generate(_neurons[i][j]);
                    }
                }
            }
            return _neurons[_layers.Length - 1];
        }

        #region Initilization.

        /// <summary>
        /// Create empty storage array for the neurons in the network.
        /// </summary>
        private void InitializeNeurons()
        {
            var neuronsList = new List<double[]>();
            for (int i = 0; i < _layers.Length; i++)
            {
                neuronsList.Add(new double[_layers[i]]);
            }
            _neurons = neuronsList.ToArray();
        }

        /// <summary>
        /// Initializes random array for the biases being held within the network.
        /// </summary>
        private void InitializeBiases()
        {
            var biasList = new List<double[]>();
            for (int i = 1; i < _layers.Length; i++)
            {
                var bias = new double[_layers[i]];
                for (int j = 0; j < _layers[i]; j++)
                {
                    bias[j] = GetRandomBias();
                }
                biasList.Add(bias);
            }
            _biases = biasList.ToArray();
        }

        /// <summary>
        /// Initializes random array for the weights being held in the network.
        /// </summary>
        private void InitializeWeights()
        {
            var weightsList = new List<double[][]>();
            for (int i = 1; i < _layers.Length; i++)
            {
                var layerWeightsList = new List<double[]>();
                int neuronsInPreviousLayer = _layers[i - 1];
                for (int j = 0; j < _layers[i]; j++)
                {
                    var neuronWeights = new double[neuronsInPreviousLayer];
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        neuronWeights[k] = GetRandomBias();
                    }
                    layerWeightsList.Add(neuronWeights);
                }
                weightsList.Add(layerWeightsList.ToArray());
            }
            _weights = weightsList.ToArray();
        }

        #endregion

        #region Backpropagation.

        public void BackPropagate(double[] inputs, double[] expected)//backpropogation;
        {
            double[] output = FeedForward(inputs);//runs feed forward to ensure neurons are populated correctly

            Cost = 0;
            for (int i = 0; i < output.Length; i++)
            {
                Cost += (double)Math.Pow(output[i] - expected[i], 2);//calculated cost of network
            }
            Cost = Cost / 2;

            double[][] gamma;

            var gammaList = new List<double[]>();
            for (int i = 0; i < _layers.Length; i++)
            {
                gammaList.Add(new double[_layers[i]]);
            }
            gamma = gammaList.ToArray(); //gamma initialization

            int layer = _layers.Length - 2;
            for (int i = 0; i < output.Length; i++)
            {
                gamma[_layers.Length - 1][i] = (output[i] - expected[i]) * _configuration.Layer(layer).ActivationFunction.Derivative(output[i]); //Gamma calculation
            }

            for (int i = 0; i < _layers[_layers.Length - 1]; i++) //calculates the w' and b' for the last layer in the network
            {
                _biases[_layers.Length - 2][i] -= gamma[_layers.Length - 1][i] * LearningRate;
                for (int j = 0; j < _layers[_layers.Length - 2]; j++)
                {

                    _weights[_layers.Length - 2][i][j] -= gamma[_layers.Length - 1][i] * _neurons[_layers.Length - 2][j] * LearningRate; //*learning 
                }
            }

            for (int i = _layers.Length - 2; i > 0; i--) //runs on all hidden layers
            {
                layer = i - 1;
                for (int j = 0; j < _layers[i]; j++) //outputs
                {
                    gamma[i][j] = 0;
                    for (int k = 0; k < gamma[i + 1].Length; k++)
                    {
                        gamma[i][j] += gamma[i + 1][k] * _weights[i][k][j];
                    }
                    gamma[i][j] *= _configuration.Layer(layer).ActivationFunction.Derivative(_neurons[i][j]); //calculate gamma
                }
                for (int j = 0; j < _layers[i]; j++) //itterate over outputs of layer
                {
                    _biases[i - 1][j] -= gamma[i][j] * LearningRate; //modify biases of network
                    for (int k = 0; k < _layers[i - 1]; k++) //itterate over inputs to layer
                    {
                        _weights[i - 1][j][k] -= gamma[i][j] * _neurons[i - 1][k] * LearningRate; //modify weights of network
                    }
                }
            }
        }

        #endregion

        #region Genetic implementation.

        /// <summary>
        /// Used as a simple mutation function for any genetic implementations.
        /// </summary>
        public void Mutate(double mutationProbability, double mutationSeverity, int randomSeed = 0)
        {
            Reseed(randomSeed);

            for (int i = 0; i < _biases.Length; i++)
            {
                for (int j = 0; j < _biases[i].Length; j++)
                {
                    _biases[i][j] = FlipCoin(mutationProbability) ? _biases[i][j] += NextDouble(-mutationSeverity, mutationSeverity) : _biases[i][j];
                }
            }

            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    for (int k = 0; k < _weights[i][j].Length; k++)
                    {
                        _weights[i][j][k] = FlipCoin(mutationProbability) ? _weights[i][j][k] += NextDouble(-mutationSeverity, mutationSeverity) : _weights[i][j][k];
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
            var nn = new NeuralNetwork(_configuration, LearningRate, _randomSeed)
            {
                Fitness = Fitness
            };

            for (int i = 0; i < _biases.Length; i++)
            {
                for (int j = 0; j < _biases[i].Length; j++)
                {
                    nn._biases[i][j] = _biases[i][j];
                }
            }
            for (int i = 0; i < _weights.Length; i++)
            {
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    for (int k = 0; k < _weights[i][j].Length; k++)
                    {
                        nn._weights[i][j][k] = _weights[i][j][k];
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
            using (TextReader tr = new StreamReader(path))
            {
                int NumberOfLines = (int)new FileInfo(path).Length;
                string[] ListLines = new string[NumberOfLines];
                int index = 1;

                for (int i = 1; i < NumberOfLines; i++)
                {
                    ListLines[i] = tr.ReadLine();
                }

                if (new FileInfo(path).Length > 0)
                {
                    for (int i = 0; i < _biases.Length; i++)
                    {
                        for (int j = 0; j < _biases[i].Length; j++)
                        {
                            _biases[i][j] = double.Parse(ListLines[index]);
                            index++;
                        }
                    }

                    for (int i = 0; i < _weights.Length; i++)
                    {
                        for (int j = 0; j < _weights[i].Length; j++)
                        {
                            for (int k = 0; k < _weights[i][j].Length; k++)
                            {
                                _weights[i][j][k] = double.Parse(ListLines[index]); ;
                                index++;
                            }
                        }
                    }
                }
                tr.Close();
            }
        }

        /// <summary>
        /// Save the biases and weights within the network to a file.
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            File.Create(path).Close();
            using (var writer = new StreamWriter(path, true))
            {
                for (int i = 0; i < _biases.Length; i++)
                {
                    for (int j = 0; j < _biases[i].Length; j++)
                    {
                        writer.WriteLine(_biases[i][j]);
                    }
                }

                for (int i = 0; i < _weights.Length; i++)
                {
                    for (int j = 0; j < _weights[i].Length; j++)
                    {
                        for (int k = 0; k < _weights[i][j].Length; k++)
                        {
                            writer.WriteLine(_weights[i][j][k]);
                        }
                    }
                }
                writer.Close();
            }
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
            return (_random.Next(0, 1000) / 1000 >= probability);
        }

        private bool FlipCoin()
        {
            return _random.Next(0, 100) >= 50;
        }

        private double GetRandomBias()
        {
            if (FlipCoin())
            {
                return (double)(_random.NextDouble() / 0.5);
            }
            return (double)((_random.NextDouble() / 0.5f) * -1);
        }

        private double NextDouble(double minimum, double maximum)
        {
            if (minimum < 0)
            {
                minimum = Math.Abs(minimum);

                if (FlipCoin())
                {
                    return (_random.NextDouble() * (maximum - minimum) + minimum) * -1;
                }
            }
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }
        #endregion
    }
}
