using Algorithms;
using Simulator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Engine
{

    public class ActorBug : BaseGraphicObject
    {
        public NeuralNetwork Brain { get; set; }

        public double MinimumTravelDistanceBeforeDamage { get; set; } = 20;
        public double MaxObserveDistance { get; set; } = 100;
        public double VisionToleranceDegrees { get; set; } = 25;
        public DateTime? LastDecisionTime { get; set; }
        public PointD LastDecisionLocation { get; set; }
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public float DecisionSensitivity { get; set; } = (float)Utility.RandomNumber(0.25, 0.55);
        public int LifeForce { get; set; } = 100; //Hitpoints? :)

        public ActorBug(Core core, NeuralNetwork brain = null)
            : base(core)
        {
            this.SetImage("../../../Images/Bug16x16.png");
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
                Brain = TrainedBrain.GetBrain();
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

            if (LastDecisionTime == null ||  (now - (DateTime)LastDecisionTime).TotalMilliseconds >= MillisecondsBetweenDecisions)
            {
                if (LastDecisionLocation != null)
                {
                    if (this.DistanceTo(LastDecisionLocation as PointD) < MinimumTravelDistanceBeforeDamage)
                    {
                        this.LifeForce--;
                    }
                }
                LastDecisionLocation = this.Location;

                var decidingFactors = GetVisionIntelligence();

                var decisions = this.Brain.FeedForward(decidingFactors);

                if (decisions[BrainOutputs.ShouldRotate] >= DecisionSensitivity)
                {
                    var rotateAmount = decisions[BrainOutputs.RotateLeftOrRightAmount];

                    if (decisions[BrainOutputs.RotateLeftOrRight] >= DecisionSensitivity)
                    {
                        this.Rotate(45 * rotateAmount);
                    }
                    else
                    {
                        this.Rotate(-45 * rotateAmount);
                    }
                }

                if (decisions[BrainOutputs.ShouldSpeedUpOrDown] >= DecisionSensitivity)
                {
                    double speedFactor = decisions[BrainOutputs.SpeedUpOrDownAmount];
                    this.Velocity.ThrottlePercentage += (speedFactor / 5.0);
                }
                else
                {
                    double speedFactor = decisions[BrainOutputs.SpeedUpOrDownAmount];
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

                LastDecisionTime = now;
            }

            if (this.IsOnScreen == false)
            {
                //Kill this bug:
                this.Visable = false;
            }

            var intersections = Intersections();

            if (intersections.Where(o=> o is not ActorBug).Count() > 0 || this.LifeForce == 0)
            {
                //Kill this bug:
                this.Visable = false;

                /* //Nah, he didnt do anything wrong.
                //Kill all coliding bugs:
                foreach (var intersection in intersections)
                {
                    intersection.Visable = false;
                }
                */
            }
        }

        /// <summary>
        /// Looks around and gets neuralnetwork inputs for visible proximity objects.
        /// </summary>
        /// <returns></returns>
        private float[] GetVisionIntelligence()
        {
            var scenerio = new float[5];

            //The closeness is expressed as a percentage of how close to the other object they are. 100% being touching 0% being 1 pixel from out of range.
            foreach (var other in Core.Actors.Collection.Where(o => o is not ActorTextBlock))
            {
                if (other == this)
                {
                    continue;
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -90))
                {
                    double distance = this.DistanceTo(other);
                    double percentageOfCloseness = 1 - (distance / MaxObserveDistance);
                    scenerio[BrainInputs.ObjTo90Left] = (float)(percentageOfCloseness > scenerio[BrainInputs.ObjTo90Left] ? percentageOfCloseness : scenerio[BrainInputs.ObjTo90Left]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -45))
                {
                    double distance = this.DistanceTo(other);
                    double percentageOfCloseness = 1 - (distance / MaxObserveDistance);
                    scenerio[BrainInputs.ObjTo45Left] = (float)(percentageOfCloseness > scenerio[BrainInputs.ObjTo45Left] ? percentageOfCloseness : scenerio[BrainInputs.ObjTo45Left]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, 0))
                {
                    double distance = this.DistanceTo(other);
                    double percentageOfCloseness = 1 - (distance / MaxObserveDistance);
                    scenerio[BrainInputs.ObjAhead] = (float)(percentageOfCloseness > scenerio[BrainInputs.ObjAhead] ? percentageOfCloseness : scenerio[BrainInputs.ObjAhead]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, +45))
                {
                    double distance = this.DistanceTo(other);
                    double percentageOfCloseness = 1 - (distance / MaxObserveDistance);
                    scenerio[BrainInputs.ObjTo45Right] = (float)(percentageOfCloseness > scenerio[BrainInputs.ObjTo45Right] ? percentageOfCloseness : scenerio[BrainInputs.ObjTo45Right]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, +90))
                {
                    double distance = this.DistanceTo(other);
                    double percentageOfCloseness = 1 - (distance / MaxObserveDistance);
                    scenerio[BrainInputs.ObjTo90Right] = (float)(percentageOfCloseness > scenerio[BrainInputs.ObjTo90Right] ? percentageOfCloseness : scenerio[BrainInputs.ObjTo90Right]);
                }
            }

            return scenerio;
        }
    }
}