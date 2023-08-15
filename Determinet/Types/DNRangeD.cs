
namespace Determinet.Types
{
    [Serializable]
    public struct DNRangeD
    {
        private double min, max;

        public double Min
        {
            get { return min; }
            set { min = value; }
        }

        public double Max
        {
            get { return max; }
            set { max = value; }
        }

        public double Length => max - min;
        public readonly double[] ToArray() => new[] { min, max };
        public static implicit operator double[](DNRangeD range) => range.ToArray();

        public DNRangeD(double min, double max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
