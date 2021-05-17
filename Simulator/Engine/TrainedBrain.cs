using Algorithms;

namespace Simulator.Engine
{
    static class BrainInputs
    {
        public const int ObjTo90Left = 0;
        public const int ObjTo45Left = 1;
        public const int ObjAhead = 2;
        public const int ObjTo45Right = 3;
        public const int ObjTo90Right = 4;
    }

    static class BrainOutputs
    {
        public const int ShouldRotate = 0;
        public const int RotateLeftOrRight = 1;
        public const int RotateLeftOrRightAmount = 2;
        public const int ShouldSpeedUpOrDown = 3;
        public const int SpeedUpOrDownAmount = 4;
    }

    public static class TrainedBrain
    {
        private static NeuralNetwork _brain = null;

        public static NeuralNetwork GetBrain()
        {
            if (_brain == null)
            {
                NeuralNetworkConfig nnConfig = new NeuralNetworkConfig();
                nnConfig.AddLayer(LayerType.Input, 5); //Vision inouts
                nnConfig.AddLayer(LayerType.Intermediate, 12, ActivationType.LeakyRelu);
                nnConfig.AddLayer(LayerType.Output, 5, ActivationType.LeakyRelu); //Movement decisions.
                _brain = new NeuralNetwork(nnConfig, 0.02f);

                for (int i = 0; i < 10000; i++)
                {
                    _brain.BackPropagate(TrainingScenerio(1, 0, 0, 0, 0), TrainingDecision(0.5f, 0.5f, 0.5f, 0, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 1, 0, 0, 0), TrainingDecision(0.5f, 0.5f, 0.5f, 0, 0));
                    _brain.BackPropagate(TrainingScenerio(1, 1, 0, 0, 0), TrainingDecision(0.5f, 0.5f, 0.5f, 0, 0));

                    //Left side detection:
                    _brain.BackPropagate(TrainingScenerio(1, 0, 0, 0, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 1, 0, 0, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(1, 1, 0, 0, 0), TrainingDecision(1, 1, 1, 1, 0));

                    //Right side detection:
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 1), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 1, 0), TrainingDecision(1, 0, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 1, 1), TrainingDecision(1, 0, 1, 1, 0));

                    //Front side detection:
                    _brain.BackPropagate(TrainingScenerio(0, 0, 1, 0, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(0, 1, 1, 1, 0), TrainingDecision(1, 1, 1, 1, 0));
                    _brain.BackPropagate(TrainingScenerio(1, 1, 1, 1, 1), TrainingDecision(1, 1, 1, 1, 0));

                    //No dection side detection:
                    _brain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 0), TrainingDecision(0.4f, 0.4f, 0.4f, 0.9f, 0.9f));
                }
            }

            return _brain.Clone();
        }

        private static float[] TrainingScenerio(float objectTo90LeftCloseness, float objectTo45LeftCloseness, float objectAheadCloseness, float objectTo45RightCloseness, float objectTo90RightCloseness)
        {
            var scenerio = new float[5];

            scenerio[BrainInputs.ObjTo90Left] = objectTo90LeftCloseness;
            scenerio[BrainInputs.ObjTo45Left] = objectTo45LeftCloseness;
            scenerio[BrainInputs.ObjAhead] = objectAheadCloseness;
            scenerio[BrainInputs.ObjTo45Right] = objectTo45RightCloseness;
            scenerio[BrainInputs.ObjTo90Right] = objectTo90RightCloseness;

            return scenerio;
        }

        private static float[] TrainingDecision(float shouldRotate, float rotateLeftOrRight, float rotateLeftOrRightAmount, float shouldSpeedUpOrDown, float speedUpOrDownAmount)
        {
            var decision = new float[5];

            decision[BrainOutputs.ShouldRotate] = shouldRotate;
            decision[BrainOutputs.RotateLeftOrRight] = rotateLeftOrRight;
            decision[BrainOutputs.RotateLeftOrRightAmount] = rotateLeftOrRightAmount;
            decision[BrainOutputs.ShouldSpeedUpOrDown] = shouldSpeedUpOrDown;
            decision[BrainOutputs.SpeedUpOrDownAmount] = speedUpOrDownAmount;

            return decision;
        }
    }
}
