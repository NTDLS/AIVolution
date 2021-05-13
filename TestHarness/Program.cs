using Algorithms;
using System;

namespace AIVolution
{
    class Program
    {
        private static NeuralNetwork nn;

        static void Main(string[] args)
        {
            Start();

            Console.ReadLine();
        }

        private static void Start()
        {
            NeuralNetworkConfig nnConfig = new NeuralNetworkConfig();

            nnConfig.AddLayer(LayerType.Input, 3);
            nnConfig.AddLayer(LayerType.Intermediate, 5, ActivationType.LeakyRelu );
            nnConfig.AddLayer(LayerType.Output, 1, ActivationType.LeakyRelu);

            nn = new NeuralNetwork(nnConfig, 0.01f);

            nn.Load("C:\\network.txt");

            //Train the network
            for (int i = 0; i < 20000; i++)
            {
                nn.BackPropagate(new float[] { 0, 0, 0 }, new float[] { 0 });
                nn.BackPropagate(new float[] { 1, 0, 0 }, new float[] { 1 });
                nn.BackPropagate(new float[] { 0, 1, 0 }, new float[] { 1 });


                nn.BackPropagate(new float[] { 0, 0, 1 }, new float[] { 1 });
                nn.BackPropagate(new float[] { 0, 0, 1 }, new float[] { 0 });
                nn.BackPropagate(new float[] { 0, 0, 1 }, new float[] { 1 });
                nn.BackPropagate(new float[] { 0, 0, 1 }, new float[] { 1 });
                nn.BackPropagate(new float[] { 0, 0, 1 }, new float[] { 0 });


                nn.BackPropagate(new float[] { 1, 1, 0 }, new float[] { 1 });
                nn.BackPropagate(new float[] { 0, 1, 1 }, new float[] { 1 });
                nn.BackPropagate(new float[] { 1, 0, 1 }, new float[] { 1 });
                nn.BackPropagate(new float[] { 1, 1, 1 }, new float[] { 1 });
            }
            Console.WriteLine($"Cost: {nn.cost:0.########}");

            nn.Save("C:\\network.txt");
            

            VerboseFeedForward(nn, new float[] { 0, 0, 0 });
            VerboseFeedForward(nn, new float[] { 1, 0, 0 });
            VerboseFeedForward(nn, new float[] { 0, 1, 0 });
            VerboseFeedForward(nn, new float[] { 0, 0, 1 });
            VerboseFeedForward(nn, new float[] { 1, 1, 0 });
            VerboseFeedForward(nn, new float[] { 0, 1, 1 });
            VerboseFeedForward(nn, new float[] { 1, 0, 1 });
            VerboseFeedForward(nn, new float[] { 1, 1, 1 });

            //We want the gate to simulate 3 input or gate (A or B or C)
            // 0 0 0    => 0
            // 1 0 0    => 1
            // 0 1 0    => 1
            // 0 0 1    => 1
            // 1 1 0    => 1
            // 0 1 1    => 1
            // 1 0 1    => 1
            // 1 1 1    => 1
        }

        private static float VerboseFeedForward(NeuralNetwork net, float[] inputs)
        {
            foreach (var val in inputs)
            {
                Console.Write($"{val:0.####},");
            }

            var result = nn.FeedForward(inputs)[0];

            Console.WriteLine("Result: {0}", result > 0.5 ? "True" : "False");

            return result;
        }
    }
}
