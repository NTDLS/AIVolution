namespace Simulator.Engine
{
    public class ActorGrass : ActorBase
    {
        public ActorGrass(Core core)
            : base(core)
        {
            SetImage("../../../Images/Grass32x32.png");
            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = 0;
            Velocity.MaxSpeed = 1;
            Velocity.ThrottlePercentage = 0;
        }
    }
}
