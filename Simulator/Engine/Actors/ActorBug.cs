using Algorithms;
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

        public ActorBug(Core core)
            : base(core)
        {
            this.SetImage("../../../Images/Bug16x16.png");
            this.Location = Core.EngineDisplay.RandomOnScreenLocation();
            this.Velocity.Angle.Degrees = Utility.RandomNumber(0, 359);
            this.Velocity.MaxSpeed = 2.5;
            this.Velocity.ThrottlePercentage = Utility.RandomNumber(0.1, 1.0);

            NeuralNetworkConfig nnConfig = new NeuralNetworkConfig();
            nnConfig.AddLayer(LayerType.Input, 3);
            nnConfig.AddLayer(LayerType.Intermediate, 5, ActivationType.LeakyRelu);
            nnConfig.AddLayer(LayerType.Output, 1, ActivationType.LeakyRelu);
            Brain = new NeuralNetwork(nnConfig, 0.01f);
        }
    }
}
