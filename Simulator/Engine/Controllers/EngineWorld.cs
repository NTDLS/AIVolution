using Determinet;
using Simulator.Engine.Actors;
using Simulator.Engine.Types;

namespace Simulator.Engine.Controllers
{
    /// <summary>
    /// Handles what the world/map looks like - populates objects.
    /// </summary>
    public class EngineWorld
    {
        private readonly EngineCore _core;

        public int Permutations { get; private set; } = -1;//How many times have we reset with a new generation.

        public EngineWorld(EngineCore core)
        {
            _core = core;
        }

        /// <summary>
        /// Called to populate the world. This is called automatically at the engine thread start but all subsequent calls will need to be made manually.
        /// </summary>
        public void Populate()
        {
            ResetMap();
        }

        /// <summary>
        /// Called after all objects have been moved and deleted objects have been purged. This is a full game tick and it a good place to apply logic.
        /// </summary>
        public void Tick()
        {
            var debugText = _core.Actors.Collection.Where(o => o.Name == "Debug Text Block").First() as ActorTextBlock;
            if (debugText != null)
            {
                debugText.Text =
                        $"Frame Rate: Avg: {_core.Display.FrameCounter.AverageFrameRate:0.0},"
                      + $" Min: {_core.Display.FrameCounter.FrameRateMin:0.0},"
                      + $" Max: {_core.Display.FrameCounter.FrameRateMax:0.0}\r\n";

                debugText.Text += "\r\nBugs Alive: " + _core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).Count();

                var bugsAlive = _core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).ToList();

                if (bugsAlive.Count > 0)
                {
                    debugText.Text += "\r\nOldest Generation: " + bugsAlive.Max(o => (o as ActorBug)?.Brain?.Fitness ?? 0);
                }
                debugText.Text += "\r\nPermutations: " + Permutations;
            }

            if (_core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).Count() <= 2)
            {
                var bestBugs = _core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).ToList();

                var greatestMinds = new List<DNNeuralNetwork>();

                foreach (var bestBug in bestBugs)
                {
                    var greatestMind = (bestBug as ActorBug)?.Brain.Clone();
                    if (greatestMind != null)
                    {
                        greatestMind.Fitness++; //Generation number;
                        greatestMinds.Add(greatestMind);
                    }
                }

                ResetMap(greatestMinds);
            }
        }

        public void WallInArena()
        {
            //Left Wall:
            for (int i = 0; i < _core.Display.VisibleSize.Height;)
            {
                var rock = new ActorRock(_core);
                rock.X = 0;
                rock.Y = i += rock.Size.Height;
                _core.Actors.Add(rock);
            }

            //Right Wall:
            for (int i = 0; i < _core.Display.VisibleSize.Height;)
            {
                var rock = new ActorRock(_core);
                rock.X = _core.Display.VisibleSize.Width - rock.Size.Width + rock.Size.Width / 2;
                rock.Y = i += rock.Size.Height;
                _core.Actors.Add(rock);
            }

            //Top Wall:
            for (int i = 0; i < _core.Display.VisibleSize.Width;)
            {
                var rock = new ActorRock(_core);
                rock.X = i += rock.Size.Width;
                rock.Y = 0;
                _core.Actors.Add(rock);
            }

            //Bottom Wall:
            for (int i = 0; i < _core.Display.VisibleSize.Width;)
            {
                var rock = new ActorRock(_core);
                rock.X = i += rock.Size.Width;
                rock.Y = _core.Display.VisibleSize.Height - rock.Size.Height;//- (rock.Size.Height / 2);
                _core.Actors.Add(rock);
            }
        }

        private void ResetMap(List<DNNeuralNetwork>? brains = null)
        {
            Permutations++;

            _core.Actors.Collection.Clear();

            WallInArena();

            for (int i = 0; i < 10; i++)
            {
                _core.Actors.Add(new ActorLava(_core));
            }

            _core.Actors.Add(new ActorInteractive(_core, "Player"));

            int brainIndex = 0;

            for (int i = 0; i < 10; i++)
            {
                DNNeuralNetwork? brain = null;

                if (brains != null && brains.Count > 0)
                {
                    brain = brains[brainIndex++]; //Split the number of brains used evenly amongst the population.
                    if (brainIndex == brains.Count)
                    {
                        brainIndex = 0;
                    }

                    //After we have added all the perfect clones, add mutated descendants.
                    if (i >= brains.Count)
                    {
                        var mutationProbability = Utility.RandomNumber(0.4, 0.6);
                        var mutationSeverity = (float)Utility.RandomNumber(-0.5, 0.5);
                        brain.Mutate(mutationProbability, mutationSeverity);
                        brain.Fitness = 0; //This is a new generation.
                    }
                }

                _core.Actors.Add(new ActorBug(_core, brain));

                _core.Actors.Add(new ActorTextBlock(_core, "Consolas", Brushes.Aqua, 10, new PointD(25, 10), true, "Debug Text Block"));
            }
        }
    }
}
