using System.Text;

namespace Determinet
{
    internal static class DniUtility
    {
        private static Random? _random = null;

        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    var seed = Guid.NewGuid().GetHashCode();
                    _random = new Random(seed);
                }
                return _random;
            }
        }

        public static int Checksum(string buffer)
        {
            return Checksum(Encoding.ASCII.GetBytes(buffer));
        }

        public static int Checksum(byte[] buffer)
        {
            int sum = 0;
            foreach (var b in buffer)
            {
                sum += (int)(sum ^ b);
            }
            return sum;
        }

        /// <summary>
        /// Flips a coin with a probability between 0.0 - 1.0.
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        public static bool FlipCoin(double probability)
        {
            return (Random.Next(0, 1000) / 1000 >= probability);
        }

        public static bool FlipCoin()
        {
            return Random.Next(0, 100) >= 50;
        }

        public static double GetRandomNeuronValue()
        {
            if (FlipCoin())
            {
                return (double)(Random.NextDouble() / 0.5);
            }
            return (double)((Random.NextDouble() / 0.5f) * -1);
        }

        public static double GetRandomBiasValue()
        {
            if (FlipCoin())
            {
                return (double)(Random.NextDouble() / 0.5);
            }
            return (double)((Random.NextDouble() / 0.5f) * -1);
        }

        public static double GetRandomWeightValue()
        {
            if (FlipCoin())
            {
                return (double)(Random.NextDouble() / 0.5);
            }
            return (double)((Random.NextDouble() / 0.5f) * -1);
        }

        public static double NextDouble(double minimum, double maximum)
        {
            if (minimum < 0)
            {
                minimum = Math.Abs(minimum);

                if (FlipCoin())
                {
                    return (Random.NextDouble() * (maximum - minimum) + minimum) * -1;
                }
            }
            return Random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
