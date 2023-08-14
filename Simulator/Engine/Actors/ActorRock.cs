namespace Simulator.Engine
{
    public class ActorRock : ActorBase
    {
        public ActorRock(Core core)
            : base(core)
        {
            SetImage("../../../Images/Rock32x32.png");
            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = 0;
            Velocity.MaxSpeed = 1;
            Velocity.ThrottlePercentage = 0;
        }
    }
}
