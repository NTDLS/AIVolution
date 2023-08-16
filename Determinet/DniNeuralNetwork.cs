using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Determinet
{
    [Serializable]
    public class DniNeuralNetwork
    {
        #region Controllers:
        [JsonProperty]
        public bool IsInitalized { get; private set; }
        #endregion

        #region Fundamental.
        [JsonProperty]
        public DniNeuralNetworkLayers Layers { get; private set; } = new();

        [JsonProperty]
        private double[][]? Neurons { get; set; }

        [JsonProperty]
        private double[][]? Biases { get; set; }

        [JsonProperty]
        private double[][][]? Weights { get; set; }
        #endregion

        #region Genetic.
        [JsonProperty]
        public double Fitness { get; set; } = 0;

        //Backprop.
        [JsonProperty]
        public double LearningRate { get; private set; } = 0.01f;

        [JsonProperty]
        public double Cost { get; private set; } = 0; //Not used in calculions, only to identify the performance of the network.

        [JsonIgnore]
        #endregion

        #region Other
        private Random _random = new();

        [JsonProperty]
        private int RandomSeed { get; set; }
        #endregion

        public void Reseed(int randomSeed = 0)
        {
            if (randomSeed == 0)
            {
                RandomSeed = Guid.NewGuid().GetHashCode();
            }
            else
            {
                RandomSeed = randomSeed;
            }

            _random = new Random(RandomSeed);
        }

        public DniNeuralNetwork(double learningRate, int randomSeed = 0)
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
                neuronsList.Add(new double[Layers[i].NodeCount]);
            }
            Neurons = neuronsList.ToArray();
        }

        private void InitializeBiases()
        {
            /// Initializes random array for the biases being held within the network.
            var biasList = new List<double[]>();
            for (int i = 1; i < Layers.Count; i++)
            {
                var bias = new double[Layers[i].NodeCount];
                for (int j = 0; j < Layers[i].NodeCount; j++)
                {
                    bias[j] = GetRandomBias();
                }
                biasList.Add(bias);
            }
            Biases = biasList.ToArray();
        }

        private void InitializeWeights()
        {
            /// Initializes random array for the weights being held in the network.
            var weightsList = new List<double[][]>();
            for (int i = 1; i < Layers.Count; i++)
            {
                var layerWeightsList = new List<double[]>();
                int neuronsInPreviousLayer = Layers[i - 1].NodeCount;
                for (int j = 0; j < Layers[i].NodeCount; j++)
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
            Weights = weightsList.ToArray();
        }

        #endregion

        #region Feed forward.

        public DniNamedInterfaceParameters FeedForward(DniNamedInterfaceParameters param)
        {
            if (IsInitalized == false)
            {
                Initialize();
            }

            var inputAliases = Layers[0].Aliases;
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

            DniNamedInterfaceParameters friendlyOutputs = new();

            var outputAliases = Layers[Layers.Count - 1].Aliases;
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
            if (Biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (Weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (Neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                Neurons[0][i] = inputs[i];
            }

            for (int i = 1; i < Layers.Count; i++)
            {
                int layer = i - 1;
                for (int j = 0; j < Layers[i].NodeCount; j++)
                {
                    double value = 0;
                    for (int k = 0; k < Layers[i - 1].NodeCount; k++)
                    {
                        value += Weights[i - 1][j][k] * Neurons[i - 1][k];
                    }

                    /*
                    var layerFeed = Layers[layer].ActivationFunction as DniIActivationOutputFeed;
                    if (layerFeed != null)
                    {
                        //We are going to pass the entire layer as well as the previous layer into this type of activation function.
                        //Note that biases are not applied, this is because it is assumed that the previous layer is a different type of activation.
                        Neurons[i] = layerFeed.Activation(Neurons[i - 1]);
                    }
                    else
                    {
                    */
                        Neurons[i][j] = Layers[layer].ActivationFunction.Activation(value + Biases[i - 1][j]);
                    /*
                    }
                    */

                    var generator = Layers[layer].ActivationFunction as DniIActivationGenerator;
                    if (generator != null)
                    {
                        Neurons[i][j] = generator.Generate(Neurons[i][j]);
                    }
                }
            }
            return Neurons[Layers.Count - 1];
        }

        #endregion

        #region Backpropagation.

        /// <summary>
        /// AI learning backpropogation by named value pairs.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="expected"></param>
        public void BackPropagate(DniNamedInterfaceParameters inputs, DniNamedInterfaceParameters expected)
        {
            BackPropagate(inputs.ToArray(), expected.ToArray());
        }


        public double[] ComputeGradientCrossEntropy(double[] targets, double[] outputs)
        {
            double[] gradient = new double[outputs.Length];

            for (int i = 0; i < outputs.Length; i++)
            {
                gradient[i] = -targets[i] / outputs[i];  // Gradient of -t*log(y)
            }

            return gradient;
        }

        /// <summary>
        /// AI learning backpropogation by named ordinal.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="expected"></param>
        /// <exception cref="Exception"></exception>
        public void BackPropagate(double[] inputs, double[] expected)
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (Biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (Weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (Neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            double[] output = FeedForward(inputs);//runs feed forward to ensure neurons are populated correctly

            Cost = 0;
            for (int i = 0; i < output.Length; i++)
            {
                Cost += (double)Math.Pow(output[i] - expected[i], 2); //calculated cost of network
            }
            Cost = Cost / 2;

            double[][] gamma;

            var gammaList = new List<double[]>();
            for (int i = 0; i < Layers.Count; i++)
            {
                gammaList.Add(new double[Layers[i].NodeCount]);
            }
            gamma = gammaList.ToArray(); //gamma initialization

            int layer = Layers.Count - 2;
            for (int i = 0; i < output.Length; i++)
            {
                gamma[Layers.Count - 1][i] = (output[i] - expected[i]) * Layers[layer].ActivationFunction.Derivative(output[i]); //Gamma calculation
            }

            for (int i = 0; i < Layers[Layers.Count - 1].NodeCount; i++) //calculates the w' and b' for the last layer in the network
            {
                Biases[Layers.Count - 2][i] -= gamma[Layers.Count - 1][i] * LearningRate;
                for (int j = 0; j < Layers[Layers.Count - 2].NodeCount; j++)
                {

                    Weights[Layers.Count - 2][i][j] -= gamma[Layers.Count - 1][i] * Neurons[Layers.Count - 2][j] * LearningRate; //*learning 
                }
            }

            for (int i = Layers.Count - 2; i > 0; i--) //runs on all hidden layers
            {
                layer = i - 1;
                for (int j = 0; j < Layers[i].NodeCount; j++) //outputs
                {
                    gamma[i][j] = 0;
                    for (int k = 0; k < gamma[i + 1].Length; k++)
                    {
                        gamma[i][j] += gamma[i + 1][k] * Weights[i][k][j];
                    }
                    gamma[i][j] *= Layers[layer].ActivationFunction.Derivative(Neurons[i][j]); //calculate gamma
                }
                for (int j = 0; j < Layers[i].NodeCount; j++) //itterate over outputs of layer
                {
                    Biases[i - 1][j] -= gamma[i][j] * LearningRate; //modify biases of network
                    for (int k = 0; k < Layers[i - 1].NodeCount; k++) //itterate over inputs to layer
                    {
                        Weights[i - 1][j][k] -= gamma[i][j] * Neurons[i - 1][k] * LearningRate; //modify weights of network
                    }
                }
            }
        }

        #endregion

        #region Genetic implementation.

        /// <summary>
        /// Simple mutation function for genetic implementations.
        /// </summary>
        public void Mutate(double mutationProbability, double mutationSeverity, int randomSeed = 0)
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (Biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (Weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }

            Reseed(randomSeed);

            for (int i = 0; i < Biases.Length; i++)
            {
                for (int j = 0; j < Biases[i].Length; j++)
                {
                    Biases[i][j] = FlipCoin(mutationProbability) ? Biases[i][j] += NextDouble(-mutationSeverity, mutationSeverity) : Biases[i][j];
                }
            }

            for (int i = 0; i < Weights.Length; i++)
            {
                for (int j = 0; j < Weights[i].Length; j++)
                {
                    for (int k = 0; k < Weights[i][j].Length; k++)
                    {
                        Weights[i][j][k] = FlipCoin(mutationProbability) ? Weights[i][j][k] += NextDouble(-mutationSeverity, mutationSeverity) : Weights[i][j][k];
                    }
                }
            }
        }

        /// <summary>
        /// Comparing For genetic implementations. Used for sorting based on the fitness of the network
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(DniNeuralNetwork other)
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
        public DniNeuralNetwork Clone()
        {
            if (IsInitalized == false)
            {
                Initialize();
            }
            if (Biases == null)
            {
                throw new Exception("Biases have not been initialized.");
            }
            if (Weights == null)
            {
                throw new Exception("Weights have not been initialized.");
            }
            if (Neurons == null)
            {
                throw new Exception("Neurons have not been initialized.");
            }

            var nn = new DniNeuralNetwork(LearningRate, RandomSeed)
            {
                Fitness = Fitness
            };

            //Clone layers:
            nn.Layers = Layers.Clone();

            //Clone biases:
            nn.Biases = new double[Biases.Length][];
            for (int i = 0; i < Biases.Length; i++)
            {
                nn.Biases[i] = new double[Biases[i].Length];
                Array.Copy(Biases[i], nn.Biases[i], Biases[i].Length);
            }

            //Clone weights:
            nn.Weights = new double[Weights.Length][][];
            for (int i = 0; i < Weights.Length; i++)
            {
                nn.Weights[i] = new double[Weights[i].Length][];
                for (int j = 0; j < Weights[i].Length; j++)
                {
                    nn.Weights[i][j] = new double[Weights[i][j].Length];
                    Array.Copy(Weights[i][j], nn.Weights[i][j], Weights[i][j].Length);
                }
            }

            //Clone neurons:
            nn.Neurons = new double[Neurons.Length][];
            for (int i = 0; i < Neurons.Length; i++)
            {
                nn.Neurons[i] = new double[Neurons[i].Length];
                Array.Copy(Neurons[i], nn.Neurons[i], Neurons[i].Length);
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
        public static DniNeuralNetwork? Load(string path)
        {
            var serialized = File.ReadAllText(path);
            var instance = JsonConvert.DeserializeObject<DniNeuralNetwork>(serialized);
            return instance;
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

            var serialized = JsonConvert.SerializeObject(this, Formatting.Indented, new StringEnumConverter());

            File.WriteAllText(path, serialized);
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
