﻿using Determinet.ActivationFunctions.Interfaces;
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
        public DniNeuralNetworkLayers Layers { get; private set; }

        #endregion

        #region Genetic.
        [JsonProperty]
        public double Fitness { get; set; } = 0;

        //Backprop.
        [JsonProperty]
        public double LearningRate { get; private set; } = 0.01f;

        [JsonProperty]
        public double Cost { get; private set; } = 0; //Not used in calculions, only to identify the performance of the network.

        #endregion

        public DniNeuralNetwork(double learningRate)
        {
            LearningRate = learningRate;
            Layers = new DniNeuralNetworkLayers(this);
        }

        #region Feed forward.

        public DniNamedInterfaceParameters FeedForward(DniNamedInterfaceParameters param)
        {

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
            for (int i = 0; i < inputs.Length; i++)
            {
                Layers[0].Neurons[i].Value = inputs[i];
            }

            for (int i = 1; i < Layers.Count; i++)
            {
                int layer = i - 1;
                for (int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    double value = 0;
                    for (int k = 0; k < Layers[i - 1].Neurons.Count; k++)
                    {
                        value += Layers[i].Neurons[j].Weights[k] * Layers[i - 1].Neurons[k].Value;
                    }

                    Layers[i].Neurons[j].Value = Layers[layer].ActivationFunction.Activation(value + Layers[i].Neurons[j].Bias);

                    var mahcine = Layers[layer].ActivationFunction as DniIActivationMachine;
                    if (mahcine != null)
                    {
                        Layers[i].Neurons[j].Value = mahcine.Generate(Layers[i].Neurons[j].Value);
                    }
                }
            }
            return Layers[Layers.Count - 1].Neurons.Select(o => o.Value).ToArray();
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

        /// <summary>
        /// AI learning backpropogation by named ordinal.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="expected"></param>
        /// <exception cref="Exception"></exception>
        public void BackPropagate(double[] inputs, double[] expected)
        {
            var output = FeedForward(inputs);//runs feed forward to ensure neurons are populated correctly

            Cost = 0;
            for (int i = 0; i < output.Length; i++)
            {
                Cost += Math.Pow(output[i] - expected[i], 2); //Calculate cost of network.
            }
            Cost /= 2;

            var gammaList = new List<double[]>();
            for (int i = 0; i < Layers.Count; i++)
            {
                gammaList.Add(new double[Layers[i].Neurons.Count]);
            }
            var gamma = gammaList.ToArray(); //gamma initialization

            for (int i = 0; i < output.Length; i++)
            {
                gamma[Layers.Count - 1][i] = (output[i] - expected[i]) * Layers[Layers.Count - 2].ActivationFunction.Derivative(output[i]); //Gamma calculation
            }

            //Calculates the "weight" and "bais" for the last layer in the network.
            for (int i = 0; i < Layers[Layers.Count - 1].Neurons.Count; i++)
            {
                Layers[Layers.Count - 2].Neurons[i].Bias -= gamma[Layers.Count - 1][i] * LearningRate;
                for (int j = 0; j < Layers[Layers.Count - 2].Neurons.Count; j++)
                {

                    Layers[Layers.Count - 1].Neurons[i].Weights[j] -= gamma[Layers.Count - 1][i] * Layers[Layers.Count - 2].Neurons[j].Value * LearningRate; //*learning 
                }
            }

            for (int i = Layers.Count - 2; i > 0; i--) //runs on all hidden layers
            {
                for (int j = 0; j < Layers[i].Neurons.Count; j++) //outputs
                {
                    gamma[i][j] = 0;
                    for (int k = 0; k < gamma[i + 1].Length; k++)
                    {
                        gamma[i][j] += gamma[i + 1][k] * Layers[i + 1].Neurons[k].Weights[j];
                    }
                    gamma[i][j] *= Layers[i - 1].ActivationFunction.Derivative(Layers[i].Neurons[j].Value); //calculate gamma
                }
                for (int j = 0; j < Layers[i].Neurons.Count; j++) //itterate over outputs of layer
                {
                    Layers[i].Neurons[j].Bias -= gamma[i][j] * LearningRate; //modify biases of network
                    for (int k = 0; k < Layers[i - 1].Neurons.Count; k++) //itterate over inputs to layer
                    {
                        Layers[i].Neurons[j].Weights[k] -= gamma[i][j] * Layers[i - 1].Neurons[k].Value * LearningRate; //modify weights of network
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
            /*
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
            */
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
            return this;
            /*
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
            */
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
            var serialized = JsonConvert.SerializeObject(this, Formatting.Indented, new StringEnumConverter());

            File.WriteAllText(path, serialized);
        }

        #endregion
    }
}
