namespace Simulator.Engine.Actors
{
    public class ActorBigShroom : ActorBase
    {
        public ActorBigShroom(EngineCore core)
            : base(core)
        {
            SetImage("../../../Images/BigShroom24x24.png");
            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = 0;
            Velocity.MaxSpeed = 1;
            Velocity.ThrottlePercentage = 0;
        }
    }
}
