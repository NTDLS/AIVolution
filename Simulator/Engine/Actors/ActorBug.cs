using Determinet;
using Determinet.Types;
using Simulator.Engine.Types;
using System;
using System.Linq;
using static Simulator.Engine.BugBrain;

namespace Simulator.Engine
{

    public class ActorBug : ActorBase
    {
        private DateTime? _lastDecisionTime;
        private PointD _lastDecisionLocation = null;

        public NeuralNetwork Brain { get; private set; }
        public double MinimumTravelDistanceBeforeDamage { get; set; } = 20;
        public double MaxObserveDistance { get; set; } = 100;
        public double VisionToleranceDegrees { get; set; } = 25;
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public double DecisionSensitivity { get; set; } = Utility.RandomNumber(0.25, 0.55);
        public int Health { get; set; } = 100;

        public ActorBug(Core core, NeuralNetwork brain = null)
            : base(core)
        {
            if (brain != null)
            {
                if (brain.Fitness > 1)
                {
                    SetImage("../../../Images/Bug32x32.png");
                }
                else if (brain.Fitness > 0)
                {
                    SetImage("../../../Images/Bug24x24.png");
                }
                else
                {
                    SetImage("../../../Images/Bug16x16.png");
                }
            }
            else
            {
                SetImage("../../../Images/Bug16x16.png");
            }

            Location = Core.Display.RandomOnScreenLocation();
            Velocity.Angle.Degrees = Utility.RandomNumber(0, 359);
            Velocity.ThrottlePercentage = Utility.RandomNumber(0.10, 0.25);

            /*
             * Added 5 input nodes to act as distance sensors.
             * We need to determine how far away any object is from each of these sensors (within some sane range)
             * and use those distances as inputs.
             * 
             * Perhaps we use the output as the rotation angle?
             */

            if (brain == null)
            {
                Brain = BugBrain.GetBrain();
            }
            else
            {
                Brain = brain.Clone();
            }
        }

        public override void ApplyIntelligence()
        {
            base.ApplyIntelligence();

            var debugBlock = Core.Actors.Collection.Where(o => o is ActorTextBlock).First() as ActorTextBlock;

            var now = DateTime.UtcNow;

            if (_lastDecisionTime == null || (now - (DateTime)_lastDecisionTime).TotalMilliseconds >= MillisecondsBetweenDecisions)
            {
                if (_lastDecisionLocation != null)
                {
                    if (DistanceTo(_lastDecisionLocation as PointD) < MinimumTravelDistanceBeforeDamage)
                    {
                        Health--;
                    }
                }
                _lastDecisionLocation = Location;

                var decidingFactors = GetVisionInputs();

                var decisions = Brain.FeedForward(decidingFactors);

                if (decisions.Get(AIOutputs.OutChangeDirection) >= DecisionSensitivity)
                {
                    var rotateAmount = decisions.Get(AIOutputs.OutRotationAmount);

                    if (decisions.Get(AIOutputs.OutRotateDirection) >= DecisionSensitivity)
                    {
                        Rotate(45 * rotateAmount);
                    }
                    else
                    {
                        Rotate(-45 * rotateAmount);
                    }
                }

                if (decisions.Get(AIOutputs.OutChangeSpeed) >= DecisionSensitivity)
                {
                    double speedFactor = decisions.Get(AIOutputs.OutChangeSpeedAmount);
                    Velocity.ThrottlePercentage += (speedFactor / 5.0);
                }
                else
                {
                    double speedFactor = decisions.Get(AIOutputs.OutChangeSpeedAmount);
                    Velocity.ThrottlePercentage += -(speedFactor / 5.0);
                }

                if (Velocity.ThrottlePercentage < 0)
                {
                    Velocity.ThrottlePercentage = 0;
                }
                if (Velocity.ThrottlePercentage == 0)
                {
                    Velocity.ThrottlePercentage = 0.10;
                }

                _lastDecisionTime = now;
            }

            if (IsOnScreen == false)
            {
                //Kill this bug:
                Visable = false;
            }

            var intersections = Intersections();

            if (intersections.Where(o => o is ActorRock || o is ActorLava).Count() > 0 || Health == 0)
            {
                //Kill this bug:
                Delete();
            }
        }

        /// <summary>
        /// Looks around and gets neuralnetwork inputs for visible proximity objects.
        /// </summary>
        /// <returns></returns>
        private AIParameters GetVisionInputs()
        {
            var aiParams = new AIParameters();

            //The closeness is expressed as a percentage of how close to the other object they are. 100% being touching 0% being 1 pixel from out-of-range.
            foreach (var other in Core.Actors.Collection.Where(o => o is not ActorTextBlock))
            {
                if (other == this)
                {
                    continue;
                }

                double distance = DistanceTo(other);
                double percentageOfCloseness = 1 - (distance / MaxObserveDistance);

                if (IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, 0))
                {
                    aiParams.SetIfLess(AIInputs.In0Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, 45))
                {
                    aiParams.SetIfLess(AIInputs.In45Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, 90))
                {
                    aiParams.SetIfLess(AIInputs.In90Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -45))
                {
                    aiParams.SetIfLess(AIInputs.In270Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -90))
                {
                    aiParams.SetIfLess(AIInputs.In315Degrees, percentageOfCloseness);
                }
            }

            return aiParams;
        }
    }
}