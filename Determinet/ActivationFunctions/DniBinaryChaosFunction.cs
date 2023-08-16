using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DniBinaryChaosFunction : DniIActivationMachine
    {
        private readonly Random _random;

        [JsonProperty]
        internal double Alpha { get; private set; } // sigmoid's alpha value

        [JsonProperty]
        internal DniRange Bounds { get; private set; } = new DniRange(-1, 1);


        [JsonProperty]
        internal int RandomSeed { get; private set; }

        public DniBinaryChaosFunction(DniNamedFunctionParameters? param)
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

        public double Produce(double x)
        {
            double y = Activation(x);
            return y > DniUtility.NextDouble(Bounds.Min, Bounds.Max) ? 1 : 0;
        }

        public double Derivative(double x)
        {
            double y = Activation(x);
            return (Alpha * y * (1 - y));
        }
    }
}
