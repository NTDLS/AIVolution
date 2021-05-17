namespace Simulator.Engine
{

    public class ActorInteractive : BaseGraphicObject
    {
        public ActorInteractive(Core core, string name)
            : base(core, name)
        {
            this.SetImage("../../../Images/Interactive24x24.png");

            this.Location = Core.Display.RandomOnScreenLocation();
            this.Velocity.Angle.Degrees = Utility.RandomNumber(0, 359);
            this.Velocity.ThrottlePercentage = 0;
        }

        public override void ApplyIntelligence()
        {
            base.ApplyIntelligence();
        }
    }
}
