namespace Simulator.Engine
{
    public class ActorSmallShroom : ActorBase
    {
        public ActorSmallShroom(Core core)
            : base(core)
        {
            SetImage("../../../Images/SmallShroom18x18.png");
            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = 0;
            Velocity.MaxSpeed = 1;
            Velocity.ThrottlePercentage = 0;
        }
    }
}
