using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Engine
{
    public class ActorLava : BaseGraphicObject
    {
        public ActorLava(Core core)
            : base(core)
        {
            this.SetImage("../../../Images/Lava32x32.png");
            this.Location = Core.EngineDisplay.RandomOnScreenLocation();
            this.Velocity.Angle.Degrees = 0;
            this.Velocity.MaxSpeed = 1;
            this.Velocity.ThrottlePercentage = 0;
        }
    }
}
