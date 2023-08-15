using Simulator.Engine.Actors;
using Simulator.Engine.Types;

namespace Simulator.Engine.Controllers
{
    /// <summary>
    /// This is the thread that all objects are advanced in. It also spawns map creation and object descruction.
    /// </summary>
    public class EngineThread
    {
        private EngineCore _core;
        private bool _keepRunning = false;
        public Thread? Handle { get; private set; }

        public int FrameAdvanceDelay { get; set; } = 25;

        public EngineThread(EngineCore core)
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

            while (Handle?.IsAlive == true)
            {
                Thread.Sleep(100);
            }
        }

        public void EngineThreadProc()
        {
            _core.World.Populate();

            while (_keepRunning)
            {
                _core.Display.FrameCounter.Calculate();

                lock (_core.DrawingSemaphore)
                {
                    if (_core.IsPaused == false)
                    {
                        AdvanceFrame();
                        _core.Actors.RemoveDeletedActors();
                        _core.World.Tick();
                    }
                }
                Thread.Sleep(FrameAdvanceDelay);
            }
        }

        /// <summary>
        /// This is where we move all objects based on their vectors.
        /// </summary>
        private void AdvanceFrame()
        {
            var player = _core.Actors.Collection.Where(o => o is ActorInteractive && o.Name == "Player").FirstOrDefault();

            if (player != null)
            {
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
            }

            //Move all objects with a throttle percentage.
            foreach (var actor in _core.Actors.Collection)
            {
                actor.X += actor.Velocity.Angle.X * actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage;
                actor.Y += actor.Velocity.Angle.Y * actor.Velocity.MaxSpeed * actor.Velocity.ThrottlePercentage;
                actor.ApplyIntelligence();
            }
        }
    }
}
