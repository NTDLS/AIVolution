namespace Simulator.Engine
{

    public class ActorInteractive : ActorBase
    {
        public ActorInteractive(Core core, string name)
            : base(core, name)
        {
            SetImage("../../../Images/Interactive24x24.png");

            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = Utility.RandomNumber(0, 359);
            Velocity.ThrottlePercentage = 0;
        }

        public override void ApplyIntelligence()
        {
            base.ApplyIntelligence();
        }
    }
}
