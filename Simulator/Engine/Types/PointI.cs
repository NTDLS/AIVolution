using System.Drawing;

namespace Simulator.Engine.Types
{
    public class PointI
    {
        public int X { get; set; }
        public int Y { get; set; }

        public PointI()
        {
        }

        public PointI(int x, int y)
        {
            X = x;
            Y = y;
        }

        public PointI(PointI p)
        {
            X = p.X;
            Y = p.Y;
        }

        public PointI(PointD p)
        {
            X = (int)p.X;
            Y = (int)p.Y;
        }

        public PointI(PointF p)
        {
            X = (int)p.X;
            Y = (int)p.Y;
        }

        public PointI(Point p)
        {
            X = p.X;
            Y = p.Y;
        }
    }
}
