namespace Determinet.Types
{
    [Serializable]
    public struct IntRange
    {
        private int min, max;

        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        public int Length
        {
            get { return max - min; }
        }

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public static implicit operator DoubleRange(IntRange range)
        {
            return new DoubleRange(range.Min, range.Max);
        }

        public static implicit operator Range(IntRange range)
        {
            return new Range(range.Min, range.Max);
        }
    }
}
