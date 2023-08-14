using Determinet;
using Determinet.Types;

namespace Simulator.Engine
{
    /// <summary>
    /// This is a pre-trained bug brain with some basic intelligence on obsticle avoidance.
    /// </summary>
    public static class BugBrain
    {
        public static class Inputs
        {
            public const int ObjTo90Left = 0;
            public const int ObjTo45Left = 1;
            public const int ObjAhead = 2;
            public const int ObjTo45Right = 3;
            public const int ObjTo90Right = 4;
        }

        public static class Outputs
        {
            public const int ShouldRotate = 0;
            public const int RotateLeftOrRight = 1;
            public const int RotateLeftOrRightAmount = 2;
            public const int ShouldSpeedUpOrDown = 3;
            public const int SpeedUpOrDownAmount = 4;
        }

        private static NeuralNetwork _brain = null;

        public static NeuralNetwork GetBrain()
        {
            if (_brain == null)
            {
                NeuralNetworkConfig nnConfig = new NeuralNetworkConfig();
                nnConfig.AddLayer(LayerType.Input, 5, ActivationType.Linear); //Vision inputs
                nnConfig.AddLayer(LayerType.Intermediate, 12, ActivationType.Linear);
                nnConfig.AddLayer(LayerType.Output, 5, ActivationType.Linear); //Movement decisions.
                _brain = new NeuralNetwork(nnConfig, 0.02f);

                for (int i = 0; i < 10000; i++)
                {
                    //Left side detection, go right.
                    _brain.BackPropagate(TrainingScenerio(1, 0, 0, 0, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 1, 0, 0, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(1, 1, 0, 0, 0), TrainingDecision(1, 1, 1, 1, 0));

                    //Right side detection, go left.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 1), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 1, 0), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 1, 1), TrainingDecision(1, 0, 1, 1, 0));

                    //Front side detection, so left or right.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 1, 0, 0), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 1, 1, 1, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(1, 1, 1, 1, 1), TrainingDecision(1, 1, 1, 1, 0));

                    //No objects dection, speed up and cruise.
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 0), TrainingDecision(0.4f, 0.4f, 0.4f, 0.9f, 0.9f));
                }
            }

            return _brain.Clone();
        }

        private static double[] TrainingScenerio(double objectTo90LeftCloseness, double objectTo45LeftCloseness, double objectAheadCloseness, double objectTo45RightCloseness, double objectTo90RightCloseness)
        {
            var scenerio = new double[5];

            scenerio[Inputs.ObjTo90Left] = objectTo90LeftCloseness;
            scenerio[Inputs.ObjTo45Left] = objectTo45LeftCloseness;
            scenerio[Inputs.ObjAhead] = objectAheadCloseness;
            scenerio[Inputs.ObjTo45Right] = objectTo45RightCloseness;
            scenerio[Inputs.ObjTo90Right] = objectTo90RightCloseness;

            return scenerio;
        }

        private static double[] TrainingDecision(double shouldRotate, double rotateLeftOrRight, double rotateLeftOrRightAmount, double shouldSpeedUpOrDown, double speedUpOrDownAmount)
        {
            var decision = new double[5];

            decision[Outputs.ShouldRotate] = shouldRotate;
            decision[Outputs.RotateLeftOrRight] = rotateLeftOrRight;
            decision[Outputs.RotateLeftOrRightAmount] = rotateLeftOrRightAmount;
            decision[Outputs.ShouldSpeedUpOrDown] = shouldSpeedUpOrDown;
            decision[Outputs.SpeedUpOrDownAmount] = speedUpOrDownAmount;

            return decision;
        }
    }
}
