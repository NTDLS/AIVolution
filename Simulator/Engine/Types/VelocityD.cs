namespace Simulator.Engine.Types
{
    public class VelocityD
    {
        public AngleD Angle { get; set; } = new AngleD();
        public double MaxSpeed { get; set; }

        public double _throttlePercentage;
        public double ThrottlePercentage
        {
            get
            {
                return _throttlePercentage;
            }
            set
            {
                _throttlePercentage = value;
                _throttlePercentage = _throttlePercentage > 1 ? 1 : _throttlePercentage;
                _throttlePercentage = _throttlePercentage < -1 ? -1 : _throttlePercentage;
            }
        }
    }
}
