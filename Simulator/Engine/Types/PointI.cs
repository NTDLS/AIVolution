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
            this.X = x;
            this.Y = y;
        }

        public PointI(PointI p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }

        public PointI(PointD p)
        {
            this.X = (int)p.X;
            this.Y = (int)p.Y;
        }

        public PointI(PointF p)
        {
            this.X = (int)p.X;
            this.Y = (int)p.Y;
        }

        public PointI(Point p)
        {
            this.X = p.X;
            this.Y = p.Y;
        }
    }
}
