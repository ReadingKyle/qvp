using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using qvp.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace qvp.StateManagement
{
    public class Debugger
    {
        private BalloonPlayer _thing;

        InputAction _up;
        InputAction _down;
        InputAction _left;
        InputAction _right;

        public Debugger (BalloonPlayer thing)
        {
            _thing = thing;
            _up = new InputAction(
                new[] { Keys.W }, false);
            _down = new InputAction(
                new[] { Keys.S }, false);
            _left = new InputAction(
                new[] { Keys.A }, false);
            _right = new InputAction(
                new[] { Keys.D }, false);
        }

        public void Update (GameTime gameTime, InputState input, PlayerIndex ControllingPlayer, PlayerIndex playerIndex)
        {
            if (_up.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _thing.Position.Y--;
            }
            if (_down.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _thing.Position.Y++;
            }
            if (_left.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _thing.Position.X--;
            }
            if (_right.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _thing.Position.X++;
            }
        }
    }
}
