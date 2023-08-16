using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System;

namespace Determinet.ActivationFunctions
{
    /*
    /// <summary>
    /// The softmax activation function is often used in the output layer of neural networks for multi-class classification tasks.
    /// It converts a vector of raw scores(logits) into a probability distribution over multiple classes.
    /// </summary>
    [Serializable]
    public class DniSoftMaxFunction : DniIActivationOutputFeed
    {
        /// <summary>
        /// Linear slope value.
        /// </summary>
        [JsonProperty]
        public double Alpha { get; set; }

        /// <summary>
        /// Function output range.
        /// </summary>
        [JsonProperty]
        public DniRange Range { get; set; }

        public DniSoftMaxFunction(DniNamedFunctionParameters? param)
        {
            if (param == null)
            {
                Alpha = 0.1;
                Range = new DniRange(-1, +1);
            }
            else
            {
                Alpha = param.Get<double>("alpha", 1);
                Range = param.Get<DniRange>("range", new DniRange(-1, +1));
            }
        }

        public double[] Activation(double[] previousLayer)
        {
            double max = previousLayer.Max();  // For numerical stability
            double[] exps = previousLayer.Select(i => Math.Exp(i - max)).ToArray();
            double sumExps = exps.Sum();
            return exps.Select(e => e / sumExps).ToArray();

            //double sumExp = previousLayer.Sum(value => Math.Exp(value));
            //return previousLayer.Select(value => Math.Exp(value) / sumExp).ToArray();
        }

        public static double[,] Derivative(double[] softmaxOutput)
        {
            int length = softmaxOutput.Length;
            double[,] jacobian = new double[length, length];

            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (i == j)
                    {
                        jacobian[i, j] = softmaxOutput[i] * (1 - softmaxOutput[i]);
                    }
                    else
                    {
                        jacobian[i, j] = -softmaxOutput[i] * softmaxOutput[j];
                    }
                }
            }
            return jacobian;
        }
    }
    */
}
