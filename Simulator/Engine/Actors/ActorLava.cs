namespace Simulator.Engine.Actors
{
    public class ActorLava : ActorBase
    {
        public ActorLava(EngineCore core)
            : base(core)
        {
            SetImage("../../../Images/Lava32x32.png");
            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = 0;
            Velocity.MaxSpeed = 1;
            Velocity.ThrottlePercentage = 0;
        }
    }
}
