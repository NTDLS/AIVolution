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

        private static NeuralNetwork _brain = null;

        public static NeuralNetwork GetBrain()
        {
            if (_brain == null)
            {
                var nnConfig = new NeuralNetworkConfig();

                nnConfig.AddInputLayer(ActivationType.Linear, //Vision inputs
                    new string[] {
                        AIInputs.In0Degrees,
                        AIInputs.In45Degrees,
                        AIInputs.In90Degrees,
                        AIInputs.In270Degrees,
                        AIInputs.In315Degrees
                    });

                nnConfig.AddIntermediateLayer(32, ActivationType.Linear);

                nnConfig.AddOutputLayer(ActivationType.Linear, //Vision inputs
                    new string[] {
                        AIOutputs.OutChangeDirection,
                        AIOutputs.OutRotateDirection,
                        AIOutputs.OutRotationAmount,
                        AIOutputs.OutChangeSpeed,
                        AIOutputs.OutChangeSpeedAmount
                    });

                _brain = new NeuralNetwork(nnConfig, 0.02f);

                for (int i = 0; i < 10000; i++)
                {
                    //Left side detection, go right.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 1, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 1), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 1, 1), TrainingDecision(1, 1, 1, 1, 0));

                    //Right side detection, go left.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 1, 0, 0), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 1, 0, 0, 0), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 1, 1, 0, 0), TrainingDecision(1, 0, 1, 1, 0));

                    //Front side detection, so left or right.
                    _brain.BackPropagate(TrainingScenerio(1, 0, 0, 0, 0), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(1, 1, 0, 0, 1), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(1, 1, 1, 1, 1), TrainingDecision(1, 1, 1, 1, 0));

                    //No objects dection, speed up and cruise.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 0), TrainingDecision(0.4f, 0.4f, 0.4f, 0.9f, 0.9f));
                }
            }

            return _brain.Clone();
        }

        private static AIParameters TrainingScenerio(double in0Degrees, double in45Degrees, double in90Degrees, double in270Degrees, double in315Degrees)
        {
            var param = new AIParameters();
            param.Set(AIInputs.In0Degrees, in0Degrees);
            param.Set(AIInputs.In45Degrees, in45Degrees);
            param.Set(AIInputs.In90Degrees, in90Degrees);
            param.Set(AIInputs.In270Degrees, in270Degrees);
            param.Set(AIInputs.In315Degrees, in315Degrees);
            return param;
        }

        private static AIParameters TrainingDecision(double outChangeDirection, double outRotateDirection, double outRotationAmount, double outChangeSpeed, double outChangeSpeedAmount)
        {
            var param = new AIParameters();
            param.Set(AIOutputs.OutChangeDirection, outChangeDirection);
            param.Set(AIOutputs.OutRotateDirection, outRotateDirection);
            param.Set(AIOutputs.OutRotationAmount, outRotationAmount);
            param.Set(AIOutputs.OutChangeSpeed, outChangeSpeed);
            param.Set(AIOutputs.OutChangeSpeedAmount, outChangeSpeedAmount);
            return param;
        }
    }
}
