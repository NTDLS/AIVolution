using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Determinet
{
    [Serializable]
    public class DniNeuralNetwork
    {
        [JsonProperty]
        public DniNeuralNetworkLayers Layers { get; private set; }

        [JsonProperty]
        public double Fitness { get; set; } = 0;

        [JsonProperty]
        public double LearningRate { get; set; } = 0.01;

        [JsonProperty]
        public double Cost { get; private set; } = 0; //Not used in calculions, only to identify the performance of the network.

        public DniNeuralNetwork(double learningRate = 0.01)
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
                for (int j = 0; j < Layers[i].Neurons.Count; j++)
                {
                    double value = 0;
                    for (int k = 0; k < Layers[i - 1].Neurons.Count; k++)
                    {
                        value += Layers[i].Neurons[j].Weights[k] * Layers[i - 1].Neurons[k].Value;
                    }

                    var activationFunction = Layers[i - 1].ActivationFunction;
                    if (activationFunction != null)
                    {
                        var activationValue = activationFunction.Activation(value + Layers[i].Neurons[j].Bias);

                        if (double.IsNaN(activationValue))
                        {
                        }

                        Layers[i].Neurons[j].Value = activationValue;
                    }

                    var producerFunction = activationFunction as DniIActivationMachine;
                    if (producerFunction != null)
                    {
                        Layers[i].Neurons[j].Value = producerFunction.Produce(Layers[i].Neurons[j].Value);
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

            var gamaActivationFunction = Layers[Layers.Count - 2].ActivationFunction;
            if (gamaActivationFunction != null)
            {
                for (int i = 0; i < output.Length; i++)
                {
                    gamma[Layers.Count - 1][i] = (output[i] - expected[i]) * gamaActivationFunction.Derivative(output[i]); //Gamma calculation
                }
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

                    var activationFunction = Layers[i - 1].ActivationFunction;
                    if (activationFunction != null)
                    {
                        gamma[i][j] *= activationFunction.Derivative(Layers[i].Neurons[j].Value); //calculate gamma
                    }
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
        /// Mutation for genetic implementations.
        /// </summary>
        public void Mutate(double mutationProbability, double mutationSeverity)
        {
            foreach (var layer in Layers.Collection)
            {
                layer.Mutate(mutationProbability, mutationSeverity);
            }
        }

        /// <summary>
        /// Create a deep-copy clone of the network.
        /// </summary>
        /// <param name="nn"></param>
        /// <returns></returns>
        public DniNeuralNetwork Clone()
        {
            return new DniNeuralNetwork
            {
                Fitness = Fitness,
                Layers = Layers.Clone(),
                Cost = Cost,
                LearningRate = LearningRate,
            };
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
            if (instance != null)
            {
                instance.Layers.Network = instance;
                foreach (var layer in instance.Layers.Collection)
                {
                    layer.Layers = instance.Layers;

                    foreach (var neuron in layer.Neurons)
                    {
                        neuron.Layer = layer;
                    }
                }
            }
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
