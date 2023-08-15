using Determinet.ActivationFunctions.Interfaces;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    [Serializable]
    public class DNBernoulliFunction : DNIActivationMachine
    {
        private readonly Random _random;

        [JsonProperty]
        internal double Alpha { get; private set; } // sigmoid's alpha value

        [JsonProperty]
        internal int RandomSeed { get; private set; }

        public DNBernoulliFunction(object[]? param)
        {
            RandomSeed = DNUtility.Checksum($"{Guid.NewGuid()}:{DateTime.Now}");
            _random = new Random(RandomSeed);

            if (param == null)
            {
                Alpha = 1;
            }
            else if (param.Length != 1)
            {
                Alpha = (double)param[0];
            }
            else
            {
                throw new ArgumentException("Invalid number of parameters supplied for BernoulliFunction.");
            }
        }

        public double Activation(double x)
        {
            return (1 / (1 + Math.Exp(-Alpha * x)));
        }

        public double Generate(double x)
        {
            double y = Activation(x);
            return y > _random.NextDouble() ? 1 : 0;
        }

        public double Derivative(double x)
        {
            double y = Activation(x);

            return (Alpha * y * (1 - y));
        }
    }
}
