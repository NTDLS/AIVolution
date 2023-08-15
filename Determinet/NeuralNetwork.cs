using Determinet.ActivationFunctions;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet
{
    public class NeuralNetwork
    {
        //Controllers:
        public NeuralNetworkLayers Layers { get; private set; } = new();
        public bool IsInitalized { get; private set; }

        //Fundamental.
        //private int[]? _layers;
        private double[][]? _neurons;
        private double[][]? _biases;
        private double[][][]? _weights;

        //Genetic.
        public double Fitness { get; set; } = 0;

        //Backprop.
        public double LearningRate { get; set; } = 0.01f;
        public double Cost { get; private set; } = 0; //Not used in calculions, only to identify the performance of the network.

        //Other.
        private Random _random = new Random();
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

        public NeuralNetwork(double learningRate, int randomSeed = 0)
        {
            Reseed(randomSeed);
            LearningRate = learningRate;
        }

        #region Initialization.

        private void Initialize()
        {
            InitializeNeurons();
            InitializeBiases();
            InitializeWeights();
            IsInitalized = true;
        }

        private void InitializeNeurons()
        {

            /// Create empty storage array for the neurons in the network.
            var neuronsList = new List<double[]>();
            for (int i = 0; i < Layers.Count; i++)
            {
                neuronsList.Add(new double[Layers.Layer(i).NodeCount]);
            }
            _neurons = neuronsList.ToArray();
        }

        private void InitializeBiases()
        {
            /// Initializes random array for the biases being held within the network.
            var biasList = new List<double[]>();
            for (int i = 1; i < Layers.Count; i++)
            {
                var bias = new double[Layers.Layer(i).NodeCount];
                for (int j = 0; j < Layers.Layer(i).NodeCount; j++)
                {
                    bias[j] = GetRandomBias();
                }
                biasList.Add(bias);
            }
            _biases = biasList.ToArray();
        }

        private void InitializeWeights()
        {
            /// Initializes random array for the weights being held in the network.
            var weightsList = new List<double[][]>();
            for (int i = 1; i < Layers.Count; i++)
            {
                var layerWeightsList = new List<double[]>();
                int neuronsInPreviousLayer = Layers.Layer(i - 1).NodeCount;
                for (int j = 0; j < Layers.Layer(i).NodeCount; j++)
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

        #region Feed forward.

        public AIParameters FeedForward(AIParameters param)
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (_biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (_weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (_neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            var inputAliases = Layers.Layer(0).Aliases;
            if (inputAliases == null)
            {
                throw new Exception("Alises are not defined for the input layer.");
            }

            double[] inputInputs = new double[inputAliases.Length];
            for (int i = 0; i < inputAliases.Length; i++)
            {
                var alias = inputAliases[i];
                inputInputs[i] = param.Get(alias, 0);
            }

            var rawOutputs = FeedForward(inputInputs);

            AIParameters friendlyOutputs = new();

            var outputAliases = Layers.Layer(Layers.Count - 1).Aliases;
            if (outputAliases == null)
            {
                throw new Exception("Alises are not defined for the output layer.");
            }

            for (int i = 0; i < outputAliases.Length; i++)
            {
                friendlyOutputs.Set(outputAliases[i], rawOutputs[i]);
            }

            return friendlyOutputs;
        }

        /// <summary>
        /// Feed forward, inputs >==> outputs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        public double[] FeedForward(double[] inputs)
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (_biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (_weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (_neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                _neurons[0][i] = inputs[i];
            }

            for (int i = 1; i < Layers.Count; i++)
            {
                int layer = i - 1;
                for (int j = 0; j < Layers.Layer(i).NodeCount; j++)
                {
                    double value = 0;
                    for (int k = 0; k < Layers.Layer(i - 1).NodeCount; k++)
                    {
                        value += _weights[i - 1][j][k] * _neurons[i - 1][k];
                    }

                    _neurons[i][j] = Layers.Layer(layer).ActivationFunction.Activation(value + _biases[i - 1][j]);

                    var mahcine = Layers.Layer(layer).ActivationFunction as IActivationMachine;
                    if (mahcine != null)
                    {
                        _neurons[i][j] = mahcine.Generate(_neurons[i][j]);
                    }
                }
            }
            return _neurons[Layers.Count - 1];
        }

        #endregion

        #region Backpropagation.

        public void BackPropagate(AIParameters inputs, AIParameters expected)
        {
            BackPropagate(inputs.ToArray(), expected.ToArray());
        }

        public void BackPropagate(double[] inputs, double[] expected)//backpropogation;
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (_biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (_weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (_neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            double[] output = FeedForward(inputs);//runs feed forward to ensure neurons are populated correctly

            Cost = 0;
            for (int i = 0; i < output.Length; i++)
            {
                Cost += (double)Math.Pow(output[i] - expected[i], 2);//calculated cost of network
            }
            Cost = Cost / 2;

            double[][] gamma;

            var gammaList = new List<double[]>();
            for (int i = 0; i < Layers.Count; i++)
            {
                gammaList.Add(new double[Layers.Layer(i).NodeCount]);
            }
            gamma = gammaList.ToArray(); //gamma initialization

            int layer = Layers.Count - 2;
            for (int i = 0; i < output.Length; i++)
            {
                gamma[Layers.Count - 1][i] = (output[i] - expected[i]) * Layers.Layer(layer).ActivationFunction.Derivative(output[i]); //Gamma calculation
            }

            for (int i = 0; i < Layers.Layer(Layers.Count - 1).NodeCount; i++) //calculates the w' and b' for the last layer in the network
            {
                _biases[Layers.Count - 2][i] -= gamma[Layers.Count - 1][i] * LearningRate;
                for (int j = 0; j < Layers.Layer(Layers.Count - 2).NodeCount; j++)
                {

                    _weights[Layers.Count - 2][i][j] -= gamma[Layers.Count - 1][i] * _neurons[Layers.Count - 2][j] * LearningRate; //*learning 
                }
            }

            for (int i = Layers.Count - 2; i > 0; i--) //runs on all hidden layers
            {
                layer = i - 1;
                for (int j = 0; j < Layers.Layer(i).NodeCount; j++) //outputs
                {
                    gamma[i][j] = 0;
                    for (int k = 0; k < gamma[i + 1].Length; k++)
                    {
                        gamma[i][j] += gamma[i + 1][k] * _weights[i][k][j];
                    }
                    gamma[i][j] *= Layers.Layer(layer).ActivationFunction.Derivative(_neurons[i][j]); //calculate gamma
                }
                for (int j = 0; j < Layers.Layer(i).NodeCount; j++) //itterate over outputs of layer
                {
                    _biases[i - 1][j] -= gamma[i][j] * LearningRate; //modify biases of network
                    for (int k = 0; k < Layers.Layer(i - 1).NodeCount; k++) //itterate over inputs to layer
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
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (_biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (_weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (_neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

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
        /// Create a deep-copy clone of the network.
        /// </summary>
        /// <param name="nn"></param>
        /// <returns></returns>
        public NeuralNetwork Clone()
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (_biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (_weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (_neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            var nn = new NeuralNetwork(LearningRate, _randomSeed)
            {
                Fitness = Fitness
            };

            //Clone layers:
            nn.Layers = Layers.Clone();

            //Clone biases:
            nn._biases = new double[_biases.Length][];
            for (int i = 0; i < _biases.Length; i++)
            {
                nn._biases[i] = new double[_biases[i].Length];
                Array.Copy(_biases[i], nn._biases[i], _biases[i].Length);
            }

            //Clone weights:
            nn._weights = new double[_weights.Length][][];
            for (int i = 0; i < _weights.Length; i++)
            {
                nn._weights[i] = new double[_weights[i].Length][];
                for (int j = 0; j < _weights[i].Length; j++)
                {
                    nn._weights[i][j] = new double[_weights[i][j].Length];
                    Array.Copy(_weights[i][j], nn._weights[i][j], _weights[i][j].Length);
                }
            }

            //Clone neurons:
            nn._neurons = new double[_neurons.Length][];
            for (int i = 0; i < _neurons.Length; i++)
            {
                nn._neurons[i] = new double[_neurons[i].Length];
                Array.Copy(_neurons[i], nn._neurons[i], _neurons[i].Length);
            }

            nn.IsInitalized = true;

            return nn;
        }

        #endregion

        #region Load / Save.

        /// <summary>
        /// Loads the biases and weights from within a file into the neural network.
        /// </summary>
        /// <param name="path"></param>
        public static NeuralNetwork? Load(string path)
        {
            var searilized = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<NeuralNetwork>(searilized);
        }

        /// <summary>
        /// Save the biases and weights within the network to a file.
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (_biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (_weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (_neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            var searilized = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, searilized);
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
