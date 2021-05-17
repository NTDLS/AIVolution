using Simulator.Engine.Types;
using System.Drawing;
using System.Threading;
using System.Linq;
using Algorithms;
using System.Collections.Generic;

namespace Simulator.Engine
{
    public class EngineThread
    {
        private Core _core;
        private bool _keepRunning = false;
        private ActorTextBlock _debugTextBlock;
        public Thread Handle { get; private set; }

        public EngineThread(Core core)
        {
            _core = core;
        }

        public void Start()
        {
            if (_keepRunning == false)
            {
                _keepRunning = true;
                Handle = new Thread(EngineThreadProc);
                Handle.Start();
            }
        }

        public void Stop()
        {
            _keepRunning = false;

            while (Handle.IsAlive)
            {
                Thread.Sleep(100);
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
                rock.X = (_core.Display.VisibleSize.Width - rock.Size.Width) + (rock.Size.Width / 2);
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
                rock.Y = (_core.Display.VisibleSize.Height - rock.Size.Height);//- (rock.Size.Height / 2);
                _core.Actors.Add(rock);
            }
        }

        private void ResetArena(List<NeuralNetwork> brains = null)
        {
            _core.Actors.Collection.Clear();

            _debugTextBlock = new ActorTextBlock(_core, "Consolas", Brushes.Aqua, 10, new PointD(5, 5), true);
            _core.Actors.Add(_debugTextBlock);

            WallInArena();

            for (int i = 0; i < 10; i++)
            {
                _core.Actors.Add(new ActorLava(_core));
            }

            int brainIndex = 0;

            for (int i = 0; i < 10; i++)
            {
                NeuralNetwork brain = null;
                
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
            }
        }

        public void EngineThreadProc()
        {
            ResetArena();

            while (_keepRunning)
            {
                _core.Display.FrameCounter.Calculate();

                lock (_core.DrawingSemaphore)
                {
                    AdvanceFrame();
                }
                Thread.Sleep(10);
            }

            //var superiorSpecimen = (_core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).First() as ActorBug);

            //superiorSpecimen.Brain.Save($"C:\\{superiorSpecimen.UID}.txt");
        }

        private void AdvanceFrame()
        {
            var player = _core.Actors.Collection[0] as ActorBug;
            var rock = _core.Actors.Collection[1] as ActorRock;

            /*
            for (int i = 0; i < 10; i++)
            {
                _core.Actors.Add(new ActorLava(_core));
            }
            */

            /*
            //Make player thrust "build up" and fade-in.
            if (_core.Input.IsKeyPressed(PlayerKey.Forward))
            {
                if (player.Velocity.ThrottlePercentage < 1.0)
                {
                    player.Velocity.ThrottlePercentage += Constants.PlayerThrustRampUp;
                }
            }
            else
            {
                //If no "forward" or "reverse" user input is received... then fade the thrust.
                if (player.Velocity.ThrottlePercentage > Constants.Limits.MinPlayerThrust)
                {
                    player.Velocity.ThrottlePercentage -= Constants.PlayerThrustRampDown;
                    if (player.Velocity.ThrottlePercentage < 0)
                    {
                        player.Velocity.ThrottlePercentage = 0;
                    }
                }
            }

            //We are going to restrict the rotation speed to a percentage of thrust.
            double rotationSpeed = player.Velocity.MaxRotationSpeed * player.Velocity.ThrottlePercentage;

            if (_core.Input.IsKeyPressed(PlayerKey.RotateCounterClockwise))
            {
                player.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
            }
            else if (_core.Input.IsKeyPressed(PlayerKey.RotateClockwise))
            {
                player.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
            }
            */

            //Move all objects with a throttle percentage.
            foreach (var actor in _core.Actors.Collection)
            {
                actor.X += (actor.Velocity.Angle.X * (actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));
                actor.Y += (actor.Velocity.Angle.Y * (actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));
                actor.ApplyIntelligence();
            }

            _debugTextBlock.Text =
                    $"Frame Rate: Avg: {_core.Display.FrameCounter.AverageFrameRate:0.0},"
                  + $" Min: {_core.Display.FrameCounter.FrameRateMin:0.0},"
                  + $" Max: {_core.Display.FrameCounter.FrameRateMax:0.0}\r\n";

            _debugTextBlock.Text += "\r\nBugs Alive: " + _core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).Count();

            var bugsAlive = _core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).ToList();

            if (bugsAlive.Count > 0)
            {
                _debugTextBlock.Text += "\r\nOldest Generation: " + bugsAlive.Max(o => (o as ActorBug).Brain.Fitness);
            }

            if (_core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).Count() <= 2)
            {
                var victorText = new ActorTextBlock(_core, "Consolas", Brushes.Red, 30, new PointD(25, 300), true);
                victorText.Text = "Superior specimen(s) identified";
                _core.Actors.Add(victorText);

                var bestBugs = _core.Actors.Collection.Where(o => o is ActorBug && o.Visable == true).ToList();

                var greatestMinds = new List<NeuralNetwork>();

                foreach (var bestBug in bestBugs)
                {
                    var greatestMind = (bestBug as ActorBug).Brain.Clone();
                    greatestMind.Fitness++; //Generation number;
                    greatestMinds.Add(greatestMind);
                }

                ResetArena(greatestMinds);
            }
        }
    }
}