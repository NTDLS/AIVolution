using Determinet.ActivationFunctions.Interfaces;
using Determinet.Types;
using Newtonsoft.Json;

namespace Determinet.ActivationFunctions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DniBinaryChaosFunction : DniIOutputFunction
    {
        private readonly Random _random;

        [JsonProperty]
        internal double Alpha { get; private set; } // sigmoid's alpha value

        [JsonProperty]
        internal DniRange Bounds { get; private set; } = new DniRange(-1, 1);

        [JsonProperty]
        internal int RandomSeed { get; private set; }

        public DniBinaryChaosFunction(DniNamedFunctionParameters param)
        {
            Alpha = param.Get<double>("alpha", 1);
            RandomSeed = param.Get<int>("RandomSeed", DniUtility.Checksum($"{Guid.NewGuid()}:{DateTime.Now}"));
            _random = new Random(RandomSeed);
        }

        public double Activation(double x)
        {
            return (1 / (1 + Math.Exp(-Alpha * x)));
        }

        public double Compute(double x)
        {
            double y = Activation(x);
            return y > NextDouble(Bounds.Min, Bounds.Max) ? 1 : 0;
        }

        public double Derivative(double x)
        {
            double y = Activation(x);
            return (Alpha * y * (1 - y));
        }

        public double NextDouble(double minimum, double maximum)
        {
            if (minimum < 0)
            {
                minimum = Math.Abs(minimum);
                if (_random.Next(0, 100) >= 50)
                {
                    return (_random.NextDouble() * (maximum - minimum) + minimum) * -1;
                }
            }
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
