using Simulator.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulator.Engine
{
    /// <summary>
    /// Handles keyboard input.
    /// </summary>
    public class EngineInput
    {
        private Core _core;
        private Dictionary<PlayerKey, KeyPressState> _keyStates = new Dictionary<PlayerKey, KeyPressState>();
        public EngineInput(Core core)
        {
            _core = core;
        }

        public bool IsKeyPressed(PlayerKey key)
        {
            if (_keyStates.ContainsKey(key))
            {
                return (_keyStates[key] == KeyPressState.Down);
            }

            return false;
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChanged(PlayerKey key, KeyPressState state, Keys? other = null)
        {
            if (_keyStates.ContainsKey(key))
            {
                _keyStates[key] = state;
            }
            else
            {
                _keyStates.Add(key, state);
            }
        }

        public void HandleSingleKeyPress(Keys key)
        {
            var player = _core.Actors.Collection.Where(o => o is ActorInteractive && o.Name == "Player").FirstOrDefault();

            if (player != null)
            {
                #region Debug.
                if (key == Keys.D0)
                {
                    player.X = _core.Display.VisibleSize.Width / 2;
                    player.Y = _core.Display.VisibleSize.Height / 2;
                    player.Velocity.Angle.Degrees = 0;
                    player.Velocity.ThrottlePercentage = 0;
                }
                else if (key == Keys.D1)
                {
                    player.Velocity.Angle.Degrees = 0;
                    player.Invalidate();
                }
                else if (key == Keys.D2)
                {
                    player.Velocity.Angle.Degrees = 45;
                    player.Invalidate();
                }
                else if (key == Keys.D3)
                {
                    player.Velocity.Angle.Degrees = 90;
                    player.Invalidate();
                }
                else if (key == Keys.D4)
                {
                    player.Velocity.Angle.Degrees = 135;
                    player.Invalidate();
                }
                else if (key == Keys.D5)
                {
                    player.Velocity.Angle.Degrees = 180;
                    player.Invalidate();
                }
                else if (key == Keys.D6)
                {
                    player.Velocity.Angle.Degrees = 225;
                    player.Invalidate();
                }
                else if (key == Keys.D7)
                {
                    player.Velocity.Angle.Degrees = 270;
                    player.Invalidate();
                }
                else if (key == Keys.D8)
                {
                    player.Velocity.Angle.Degrees = 315;
                    player.Invalidate();
                }
                else if (key == Keys.D9)
                {
                }
                else if (key == Keys.F1)
                {
                }
                else if (key == Keys.F12)
                {
                    //_core.ShowDebug = !_core.ShowDebug;
                }
                else if (key == Keys.Escape)
                {
                    _core.Stop();
                }
            }
            #endregion
        }
    }

}
