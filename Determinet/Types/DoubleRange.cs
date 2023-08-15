
namespace Determinet.Types
{
    [Serializable]
    public struct DoubleRange
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

        public double Length
        {
            get { return max - min; }
        }

        public DoubleRange(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        public readonly double[] ToArray()
        {
            return new[] { min, max };
        }

        public static implicit operator double[](DoubleRange range)
        {
            return range.ToArray();
        }
    }
}
