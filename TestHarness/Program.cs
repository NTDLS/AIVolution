using Determinet;
using Determinet.Types;
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
            var nnConfig = new NeuralNetworkConfig();

            nnConfig.AddInputLayer(3, ActivationType.Linear);
            nnConfig.AddIntermediateLayer(5, ActivationType.Linear);
            nnConfig.AddLinearIntermediateLayer(4, 1, new DoubleRange(-1, 1));
            nnConfig.AddOutputLayer(1, ActivationType.Linear);

            nn = new NeuralNetwork(nnConfig, 0.01f);

            nn.Load("C:\\network.txt");

            //Train the network
            for (int i = 0; i < 20000; i++)
            {
                nn.BackPropagate(new double[] { 0, 0, 0 }, new double[] { 0 });
                nn.BackPropagate(new double[] { 1, 0, 0 }, new double[] { 1 });
                nn.BackPropagate(new double[] { 0, 1, 0 }, new double[] { 1 });


                nn.BackPropagate(new double[] { 0, 0, 1 }, new double[] { 1 });
                nn.BackPropagate(new double[] { 0, 0, 1 }, new double[] { 0 });
                nn.BackPropagate(new double[] { 0, 0, 1 }, new double[] { 1 });
                nn.BackPropagate(new double[] { 0, 0, 1 }, new double[] { 1 });
                nn.BackPropagate(new double[] { 0, 0, 1 }, new double[] { 0 });


                nn.BackPropagate(new double[] { 1, 1, 0 }, new double[] { 1 });
                nn.BackPropagate(new double[] { 0, 1, 1 }, new double[] { 1 });
                nn.BackPropagate(new double[] { 1, 0, 1 }, new double[] { 1 });
                nn.BackPropagate(new double[] { 1, 1, 1 }, new double[] { 1 });
            }
            Console.WriteLine($"Cost: {nn.Cost:0.########}");

            nn.Save("C:\\network.txt");


            VerboseFeedForward(nn, new double[] { 0, 0, 0 });
            VerboseFeedForward(nn, new double[] { 1, 0, 0 });
            VerboseFeedForward(nn, new double[] { 0, 1, 0 });
            VerboseFeedForward(nn, new double[] { 0, 0, 1 });
            VerboseFeedForward(nn, new double[] { 1, 1, 0 });
            VerboseFeedForward(nn, new double[] { 0, 1, 1 });
            VerboseFeedForward(nn, new double[] { 1, 0, 1 });
            VerboseFeedForward(nn, new double[] { 1, 1, 1 });

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

        private static double VerboseFeedForward(NeuralNetwork net, double[] inputs)
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
