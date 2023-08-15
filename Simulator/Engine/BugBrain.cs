using Determinet;
using Determinet.Types;

namespace Simulator.Engine
{
    /// <summary>
    /// This is a pre-trained bug brain with some basic intelligence on obsticle avoidance.
    /// </summary>
    public static class BugBrain
    {
        public static class AIInputs
        {
            public const string In0Degrees = "In0Degrees";
            public const string In45Degrees = "In45Degrees";
            public const string In90Degrees = "In90Degrees";
            public const string In270Degrees = "In270Degrees";
            public const string In315Degrees = "In315Degrees";
        }

        public static class AIOutputs
        {
            public const string OutChangeDirection = "OutChangeDirection";
            public const string OutRotateDirection = "OutRotateDirection";
            public const string OutRotationAmount = "OutRotationAmount";
            public const string OutChangeSpeed = "OutChangeSpeed";
            public const string OutChangeSpeedAmount = "OutChangeSpeedAmount";
        }

        private static DNNeuralNetwork? _brain = null;

        public static DNNeuralNetwork GetBrain()
        {
            if (_brain == null)
            {
                string fileName = ".\\bugbrain.txt";

                if (File.Exists(fileName))
                {
                    _brain = DNNeuralNetwork.Load(fileName);
                    if (_brain != null)
                    {
                        return _brain.Clone();
                    }
                }

                _brain = new DNNeuralNetwork(0.09f);

                _brain.Layers.AddInputLayer(ActivationType.Sigmoid, //Vision inputs
                    new string[] {
                        AIInputs.In0Degrees,
                        AIInputs.In45Degrees,
                        AIInputs.In90Degrees,
                        AIInputs.In270Degrees,
                        AIInputs.In315Degrees
                    });

                _brain.Layers.AddIntermediateLayer(48, ActivationType.Sigmoid);
                _brain.Layers.AddIntermediateLayer(32, ActivationType.Sigmoid);
                _brain.Layers.AddIntermediateLayer(16, ActivationType.Sigmoid);

                _brain.Layers.AddOutputLayer(ActivationType.Sigmoid, //Decision outputs
                    new string[] {
                        AIOutputs.OutChangeDirection,
                        AIOutputs.OutRotateDirection,
                        AIOutputs.OutRotationAmount,
                        AIOutputs.OutChangeSpeed,
                        AIOutputs.OutChangeSpeedAmount
                    });

                for (int i = 0; i < 5000; i++)
                {
                    //Left side detection, go right.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 2, 0), TrainingDecision(2, 2, 2, 2, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 2), TrainingDecision(2, 2, 2, 2, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 2, 2), TrainingDecision(2, 2, 2, 2, 0));

                    //Right side detection, go left.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 2, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 2, 0, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 2, 2, 0, 0), TrainingDecision(2, 0, 2, 2, 0));

                    //Front side detection, so left or right.
                    _brain.BackPropagate(TrainingScenerio(2, 0, 0, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                    _brain.BackPropagate(TrainingScenerio(2, 2, 0, 0, 2), TrainingDecision(2, 2, 2, 2, 0));
                    _brain.BackPropagate(TrainingScenerio(2, 2, 2, 2, 2), TrainingDecision(2, 2, 2, 2, 0));

                    //No objects dection, speed up and cruise.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 0), TrainingDecision(0.4f, 0.4f, 0.4f, 0.9f, 0.9f));
                }
                _brain.Save(fileName);
            }

            return _brain.Clone();
        }

        private static DNNamedParameter TrainingScenerio(double in0Degrees, double in45Degrees, double in90Degrees, double in270Degrees, double in315Degrees)
        {
            var param = new DNNamedParameter();
            param.Set(AIInputs.In0Degrees, in0Degrees);
            param.Set(AIInputs.In45Degrees, in45Degrees);
            param.Set(AIInputs.In90Degrees, in90Degrees);
            param.Set(AIInputs.In270Degrees, in270Degrees);
            param.Set(AIInputs.In315Degrees, in315Degrees);
            return param;
        }

        private static DNNamedParameter TrainingDecision(double outChangeDirection, double outRotateDirection, double outRotationAmount, double outChangeSpeed, double outChangeSpeedAmount)
        {
            var param = new DNNamedParameter();
            param.Set(AIOutputs.OutChangeDirection, outChangeDirection);
            param.Set(AIOutputs.OutRotateDirection, outRotateDirection);
            param.Set(AIOutputs.OutRotationAmount, outRotationAmount);
            param.Set(AIOutputs.OutChangeSpeed, outChangeSpeed);
            param.Set(AIOutputs.OutChangeSpeedAmount, outChangeSpeedAmount);
            return param;
        }
    }
}
