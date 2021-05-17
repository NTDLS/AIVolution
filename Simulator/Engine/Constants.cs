using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Engine
{
    public static class Constants
    {
        public const double PlayerThrustRampUp = 0.05;
        public const double PlayerThrustRampDown = 0.01;

        public static class Limits
        {
            public const double MaxPlayerSpeed = 5.0;

            public const double MaxRotationSpeed = 3.0;

            public const double MinPlayerThrust = 0; //0.25;

            public const int MinSpeed = 3;
            public const int MaxSpeed = 7;

            public const double FrameLimiter = 80.0; //80.0 seems to be a good rate.
        }
    }
}
