namespace Simulator.Engine.Types
{
    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD()
        {
        }

        public static double DistanceTo(PointD from, PointD to)
        {
            var deltaX = Math.Pow((to.X - from.X), 2);
            var deltaY = Math.Pow((to.Y - from.Y), 2);
            return Math.Sqrt(deltaY + deltaX);
        }

        public static double AngleTo(PointD from, PointD to)
        {
            var fRadians = Math.Atan2((to.Y - from.Y), (to.X - from.X));
            var fDegrees = ((AngleD.RadiansToDegrees(fRadians) + 360.0) + AngleD.DegreeOffset) % 360.0;
            return fDegrees;
        }

        #region  Unary Operator Overloading.

        public static PointD operator -(PointD original, PointD modifier)
        {
            return new PointD(original.X - modifier.X, original.Y - modifier.Y);
        }

        public static PointD operator -(PointD original, double modifier)
        {
            return new PointD(original.X - modifier, original.Y - modifier);
        }

        public static PointD operator +(PointD original, PointD modifier)
        {
            return new PointD(original.X + modifier.X, original.Y + modifier.Y);
        }

        public static PointD operator +(PointD original, double modifier)
        {
            return new PointD(original.X + modifier, original.Y + modifier);
        }

        public static PointD operator *(PointD original, PointD modifier)
        {
            return new PointD(original.X * modifier.X, original.Y * modifier.Y);
        }

        public static PointD operator *(PointD original, double modifier)
        {
            return new PointD(original.X * modifier, original.Y * modifier);
        }

        public override bool Equals(object? o)
        {
            if (o == null || o is not AngleD)
            {
                return false;
            }

            return (Math.Round(((PointD)o).X, 4) == X && Math.Round(((PointD)o).Y, 4) == Y);
        }

        public static bool operator !=(PointD? lhs, PointD? rhs) => !(lhs == rhs);

        public static bool operator ==(PointD? lhs, PointD? rhs)
        {
            if (object.ReferenceEquals(lhs, null))
            {
                return object.ReferenceEquals(rhs, null);
            }

            if (object.ReferenceEquals(rhs, null))
            {
                return object.ReferenceEquals(lhs, null);
            }

            return lhs.Equals(rhs);
        }

        #endregion

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round(X, 4).ToString("#.####")},{Math.Round(Y, 4).ToString("#.####")}}}";
        }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointD(PointD p)
        {
            X = p.X;
            Y = p.Y;
        }

        public PointD(PointF p)
        {
            X = p.X;
            Y = p.Y;
        }

        public PointD(Point p)
        {
            X = p.X;
            Y = p.Y;
        }
    }
}