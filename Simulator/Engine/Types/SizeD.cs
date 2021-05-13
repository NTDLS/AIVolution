namespace Simulator.Engine.Types
{
    public class SizeD
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public SizeD()
        {

        }

        public SizeD(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
