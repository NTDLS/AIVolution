using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    [Serializable]
    public class DniBernoulliFunction : DniIActivationMachine
    {
        private readonly Random _random;

        [JsonProperty]
        internal double Alpha { get; private set; } // sigmoid's alpha value

        [JsonProperty]
        internal int RandomSeed { get; private set; }

        public DniBernoulliFunction(DniNamedFunctionParameters? param)
        {

            RandomSeed = DniUtility.Checksum($"{Guid.NewGuid()}:{DateTime.Now}");
            _random = new Random(RandomSeed);

            if (param == null)
            {
                Alpha = 1;
            }
            else
            {
                Alpha = param.Get<double>("alpha", 1);
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
