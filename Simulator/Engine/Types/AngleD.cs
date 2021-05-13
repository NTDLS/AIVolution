using System;
using System.Numerics;
using System.Windows;

namespace Simulator.Engine.Types
{
    public class AngleD
    {
        #region Static Utilities.

        /// <summary>
        /// Rotate the angle counter-clockwise by 90 degrees. All of our graphics math should assume this.
        /// </summary>
        public static double DegreeOffset = 90.0;
        public static double RadianOffset = (Math.PI / 180) * DegreeOffset; //1.5707963267948966

        const double DEG_TO_RAD = Math.PI / 180.0;
        const double RAD_TO_DEG = 180.0 / Math.PI;

        public static double RadiansToDegrees(double rad)
        {
            return rad * RAD_TO_DEG;
        }

        public static double DegreesToRadians(double deg)
        {
            return deg * DEG_TO_RAD;
        }

        public static double XYToRadians(double x, double y)
        {
            return Math.Atan2(y, x) + RadianOffset;
        }

        public static double XYToDegrees(double x, double y)
        {
            return AngleD.RadiansToDegrees(Math.Atan2(y, x)) + DegreeOffset;
        }

        public static PointD ToXY(AngleD angle)
        {
            return new PointD(angle.X, angle.Y);
        }

        public static PointD DegreesToXY(double degrees)
        {
            double radians = AngleD.DegreesToRadians(degrees) - RadianOffset;
            return new PointD(Math.Cos(radians), Math.Sin(radians));
        }

        public static PointD RadiansToXY(double radians)
        {
            radians -= RadianOffset;
            return new PointD(Math.Cos(radians), Math.Sin(radians));
        }

        #endregion

        #region ~/CTor.

        public AngleD()
        {
        }

        public AngleD(AngleD angle)
        {
            Degrees = angle.Degrees;
        }

        public AngleD(double degrees)
        {
            Degrees = degrees;
        }

        public AngleD(double x, double y)
        {
            Degrees = AngleD.RadiansToDegrees(Math.Atan2(y, x)) + DegreeOffset;
        }

        #endregion

        #region  Unary Operator Overloading.

        public static AngleD operator -(AngleD original, AngleD modifier)
        {
            return new AngleD(original.Degrees - modifier.Degrees);
        }

        public static AngleD operator -(AngleD original, double degrees)
        {
            return new AngleD(original.Degrees - degrees);
        }

        public static AngleD operator +(AngleD original, AngleD modifier)
        {
            return new AngleD(original.Degrees + modifier.Degrees);
        }

        public static AngleD operator +(AngleD original, double degrees)
        {
            return new AngleD(original.Degrees + degrees);
        }

        public static AngleD operator *(AngleD original, AngleD modifier)
        {
            return new AngleD(original.Degrees * modifier.Degrees);
        }

        public static AngleD operator *(AngleD original, double degrees)
        {
            return new AngleD(original.Degrees * degrees);
        }

        public override bool Equals(object o)
        {
            return (Math.Round(((AngleD)o).X, 4) == this.X && Math.Round(((AngleD)o).Y, 4) == this.Y);
        }

        #endregion

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{{{Math.Round(X, 4):#.####}x,{Math.Round(Y, 4):#.####}y}}";
        }
        
        public double _degrees = 0;
        public double Degrees
        {
            get
            {
                return _degrees;
            }
            set
            {
                if (value < 0)
                {
                    _degrees = (360 - (Math.Abs(value) % 360.0));
                }
                else
                {
                    _degrees = value % 360;
                }
            }
        }

        public double Radians
        {
            get
            {
                return AngleD.DegreesToRadians(_degrees) - RadianOffset;
            }
        }

        public double RadiansUnadjusted
        {
            get
            {
                return AngleD.DegreesToRadians(_degrees);
            }
        }

        public double X
        {
            get
            {
                return Math.Cos(Radians);
            }
        }

        public double Y
        {
            get
            {
                return Math.Sin(Radians);
            }
        }
    }
}
