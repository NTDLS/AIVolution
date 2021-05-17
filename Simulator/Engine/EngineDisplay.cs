using Simulator.Engine.Types;
using System.Drawing;
using System.Windows.Forms;

namespace Simulator.Engine
{
    /// <summary>
    /// Handles all matters related to screen metrics.
    /// </summary>
    public class EngineDisplay
    {
        private Core _core;
        public FrameCounter FrameCounter { get; set; } = new FrameCounter();
        public RectangleF VisibleBounds { get; private set; }

        private Size _visibleSize;
        public Size VisibleSize
        {
            get
            {
                return _visibleSize;
            }
        }

        private Control _drawingSurface;
        public Control DrawingSurface
        {
            get
            {
                return _drawingSurface;
            }
        }

        public PointD RandomOnScreenLocation()
        {
            return new PointD(Utility.Random.Next(0, VisibleSize.Width), Utility.Random.Next(0, VisibleSize.Height));
        }

        public PointD RandomOffScreenLocation(int min = 100, int max = 500)
        {
            double x;
            double y;

            if (Utility.FlipCoin())
            {
                if (Utility.FlipCoin())
                {
                    x = -Utility.RandomNumber(min, max);
                    y = Utility.RandomNumber(0, VisibleSize.Height);
                }
                else
                {
                    y = -Utility.RandomNumber(min, max);
                    x = Utility.RandomNumber(0, VisibleSize.Width);
                }
            }
            else
            {
                if (Utility.FlipCoin())
                {
                    x = VisibleSize.Width + Utility.RandomNumber(min, max);
                    y = Utility.RandomNumber(0, VisibleSize.Height);
                }
                else
                {
                    y = VisibleSize.Height + Utility.RandomNumber(min, max);
                    x = Utility.RandomNumber(0, VisibleSize.Width);
                }

            }

            return new PointD(x, y);
        }

        public EngineDisplay(Core core, Control drawingSurface, Size visibleSize)
        {
            _core = core;
            _drawingSurface = drawingSurface;
            _visibleSize = visibleSize;
            VisibleBounds = new RectangleF(0, 0, visibleSize.Width, visibleSize.Height);
        }
    }
}
