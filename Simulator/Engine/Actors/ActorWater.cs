namespace Simulator.Engine
{
    public class ActorWater : ActorBase
    {
        public ActorWater(Core core)
            : base(core)
        {
            SetImage("../../../Images/Water32x32.png");
            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = 0;
            Velocity.MaxSpeed = 1;
            Velocity.ThrottlePercentage = 0;
        }
    }
}
