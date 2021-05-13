using Simulator.Engine.Types;
using System.Drawing;
using System.Threading;

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

        public void EngineThreadProc()
        {
            _core.Actors.Add(new ActorWater(_core));
            _core.Actors.Add(new ActorBigShroom(_core));
            _core.Actors.Add(new ActorGrass(_core));
            _core.Actors.Add(new ActorRock(_core));
            _core.Actors.Add(new ActorLava(_core));
            _core.Actors.Add(new ActorSmallShroom(_core));

            for (int i = 0; i < 25; i++)
            {
                _core.Actors.Add(new ActorBug(_core));
            }

            _debugTextBlock = new ActorTextBlock(_core, "Consolas", Brushes.Aqua, 10, new PointD(5, 5), true);
            _core.Actors.Add(_debugTextBlock);

            while (_keepRunning)
            {
                _core.EngineDisplay.FrameCounter.Calculate();

                lock (_core.DrawingSemaphore)
                {
                    AdvanceFrame();
                }

                Thread.Sleep(10);
            }
        }

        private void AdvanceFrame()
        {
            int actorOrdinal = 0;
            foreach (var actor in _core.Actors.Collection)
            {
                var intersections = actor.Intersections();

                if (intersections.Count > 0)
                {
                }

                if (actor is ActorBug)
                {
                    if (intersections.Count == 0)
                    {
                        if (actorOrdinal % 2 == 0)
                        {
                            actor.Velocity.Angle.Degrees += 0.5;
                        }
                        else
                        {
                            actor.Velocity.Angle.Degrees -= 0.5;
                        }

                        actor.X += (actor.Velocity.Angle.X * (actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));
                        actor.Y += (actor.Velocity.Angle.Y * (actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));
                    }
                }
                else
                {
                    actor.X += (actor.Velocity.Angle.X * (actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));
                    actor.Y += (actor.Velocity.Angle.Y * (actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage));
                }
            }

            _debugTextBlock.Text =
                    $"Frame Rate: Avg: {_core.EngineDisplay.FrameCounter.AverageFrameRate:0.0},"
                  + $" Min: {_core.EngineDisplay.FrameCounter.FrameRateMin:0.0},"
                  + $" Max: {_core.EngineDisplay.FrameCounter.FrameRateMax:0.0}\r\n";

            actorOrdinal++;
        }
    }
}
