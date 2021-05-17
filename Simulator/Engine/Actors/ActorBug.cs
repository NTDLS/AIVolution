using Algorithms;
using Simulator.Engine.Types;
using System;
using System.Linq;

namespace Simulator.Engine
{

    public class ActorBug : BaseGraphicObject
    {
        private DateTime? _lastDecisionTime;
        private PointD _lastDecisionLocation = null;

        public NeuralNetwork Brain { get; private set; }
        public double MinimumTravelDistanceBeforeDamage { get; set; } = 20;
        public double MaxObserveDistance { get; set; } = 100;
        public double VisionToleranceDegrees { get; set; } = 25;
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public float DecisionSensitivity { get; set; } = (float)Utility.RandomNumber(0.25, 0.55);
        public int Health { get; set; } = 100;

        public ActorBug(Core core, NeuralNetwork brain = null)
            : base(core)
        {
            if (brain != null)
            {
                if (brain.Fitness > 1)
                {
                    this.SetImage("../../../Images/Bug32x32.png");
                }
                else if (brain.Fitness > 0)
                {
                    this.SetImage("../../../Images/Bug24x24.png");
                }
                else
                {
                    this.SetImage("../../../Images/Bug16x16.png");
                }
            }
            else
            {
                this.SetImage("../../../Images/Bug16x16.png");
            }

            this.Location = Core.Display.RandomOnScreenLocation();
            this.Velocity.Angle.Degrees = Utility.RandomNumber(0, 359);
            this.Velocity.ThrottlePercentage = Utility.RandomNumber(0.10, 0.25);

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

            DateTime now = DateTime.UtcNow;

            if (_lastDecisionTime == null ||  (now - (DateTime)_lastDecisionTime).TotalMilliseconds >= MillisecondsBetweenDecisions)
            {
                if (_lastDecisionLocation != null)
                {
                    if (this.DistanceTo(_lastDecisionLocation as PointD) < MinimumTravelDistanceBeforeDamage)
                    {
                        this.Health--;
                    }
                }
                _lastDecisionLocation = this.Location;

                var decidingFactors = GetVisionInputs();

                var decisions = this.Brain.FeedForward(decidingFactors);

                if (decisions[BugBrain.Outputs.ShouldRotate] >= DecisionSensitivity)
                {
                    var rotateAmount = decisions[BugBrain.Outputs.RotateLeftOrRightAmount];

                    if (decisions[BugBrain.Outputs.RotateLeftOrRight] >= DecisionSensitivity)
                    {
                        this.Rotate(45 * rotateAmount);
                    }
                    else
                    {
                        this.Rotate(-45 * rotateAmount);
                    }
                }

                if (decisions[BugBrain.Outputs.ShouldSpeedUpOrDown] >= DecisionSensitivity)
                {
                    double speedFactor = decisions[BugBrain.Outputs.SpeedUpOrDownAmount];
                    this.Velocity.ThrottlePercentage += (speedFactor / 5.0);
                }
                else
                {
                    double speedFactor = decisions[BugBrain.Outputs.SpeedUpOrDownAmount];
                    this.Velocity.ThrottlePercentage += -(speedFactor / 5.0);
                }

                if (this.Velocity.ThrottlePercentage < 0)
                {
                    this.Velocity.ThrottlePercentage = 0;
                }
                if (this.Velocity.ThrottlePercentage == 0)
                {
                    this.Velocity.ThrottlePercentage = 0.10;
                }

                _lastDecisionTime = now;
            }

            if (this.IsOnScreen == false)
            {
                //Kill this bug:
                this.Visable = false;
            }

            var intersections = Intersections();

            if (intersections.Where(o=> o is ActorRock || o is ActorLava).Count() > 0 || this.Health == 0)
            {
                //Kill this bug:
                this.Delete();
            }
        }

        /// <summary>
        /// Looks around and gets neuralnetwork inputs for visible proximity objects.
        /// </summary>
        /// <returns></returns>
        private float[] GetVisionInputs()
        {
            var scenerio = new float[5];

            //The closeness is expressed as a percentage of how close to the other object they are. 100% being touching 0% being 1 pixel from out of range.
            foreach (var other in Core.Actors.Collection.Where(o => o is not ActorTextBlock))
            {
                if (other == this)
                {
                    continue;
                }

                double distance = this.DistanceTo(other);
                double percentageOfCloseness = 1 - (distance / MaxObserveDistance);

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -90))
                {
                    scenerio[BugBrain.Inputs.ObjTo90Left] = (float)
                        (percentageOfCloseness > scenerio[BugBrain.Inputs.ObjTo90Left] ? percentageOfCloseness : scenerio[BugBrain.Inputs.ObjTo90Left]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -45))
                {
                    scenerio[BugBrain.Inputs.ObjTo45Left] = (float)
                        (percentageOfCloseness > scenerio[BugBrain.Inputs.ObjTo45Left] ? percentageOfCloseness : scenerio[BugBrain.Inputs.ObjTo45Left]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, 0))
                {
                    scenerio[BugBrain.Inputs.ObjAhead] = (float)
                        (percentageOfCloseness > scenerio[BugBrain.Inputs.ObjAhead] ? percentageOfCloseness : scenerio[BugBrain.Inputs.ObjAhead]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, +45))
                {
                    scenerio[BugBrain.Inputs.ObjTo45Right] = (float)
                        (percentageOfCloseness > scenerio[BugBrain.Inputs.ObjTo45Right] ? percentageOfCloseness : scenerio[BugBrain.Inputs.ObjTo45Right]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, +90))
                {
                    scenerio[BugBrain.Inputs.ObjTo90Right] = (float)
                        (percentageOfCloseness > scenerio[BugBrain.Inputs.ObjTo90Right] ? percentageOfCloseness : scenerio[BugBrain.Inputs.ObjTo90Right]);
                }
            }

            return scenerio;
        }
    }
}