using Determinet;
using Determinet.Types;

namespace AIVolution
{
    class Program
    {
        static void Main()
        {
            Start();
            Console.ReadLine();
        }

        private static void Start()
        {
            var nn = new DniNeuralNetwork(0.01f);
            nn.Layers.AddInput(ActivationType.Linear, 3);
            nn.Layers.AddIntermediate(ActivationType.Linear, 5);

            DniNamedFunctionParameters param = new();
            param.Set("alpha", 1);
            param.Set("range", new DniRange(-1, 1));

            nn.Layers.AddIntermediate(ActivationType.Linear, 4, param);
            nn.Layers.AddOutput(1);

            //Train the network
            for (int i = 0; i < 10000; i++)
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

        private static double VerboseFeedForward(DniNeuralNetwork net, double[] inputs)
        {
            foreach (var val in inputs)
            {
                Console.Write($"{val:0.####},");
            }

            var result = net.FeedForward(inputs)[0];

            Console.WriteLine("Result: {0}", result > 0.5 ? "True" : "False");

            return result;
        }
    }
}
