namespace Simulator.Engine.Actors
{
    public class ActorWater : ActorBase
    {
        public ActorWater(EngineCore core)
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
