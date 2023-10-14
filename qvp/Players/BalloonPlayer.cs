using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class BalloonPlayer
    {
        private Texture2D _balloonTexture;
        private Texture2D _stringTexture;
        public Vector2 Position;
        public int NumPumps;

        private int _balloonAnimationFrame;
        private int _stringAnimationFrame;
        private double _animationTimer;

        public bool IsFloating;
        public bool IsFinished;

        private Vector2 _velocity;

        private bool _stringForward;

        public BalloonPlayer(Texture2D balloonTexture, Texture2D stringTexture, Vector2 position) 
        {
            _balloonTexture = balloonTexture;
            _stringTexture = stringTexture;
            Position = position;
            _velocity = new Vector2(0, 510);
            IsFinished = false;
            _stringForward = true;
        }

        public void Draw (SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsFloating && (480 + (NumPumps * -100)) < Position.Y && NumPumps > 0) Position.Y -= _velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (IsFloating) IsFinished = true;

            if (NumPumps >= 0 && NumPumps < 10) _balloonAnimationFrame = 0;
            else if (NumPumps >= 10 && NumPumps < 20) _balloonAnimationFrame = 1;
            else if (NumPumps >= 20 && NumPumps < 30) _balloonAnimationFrame = 2;
            else if (NumPumps >= 30 && NumPumps < 40) _balloonAnimationFrame = 3;
            else if (NumPumps >= 40 && NumPumps < 50) _balloonAnimationFrame = 4;
            else if (NumPumps >= 50) _balloonAnimationFrame = 5;
            spriteBatch.Draw(_balloonTexture, Position, new Rectangle(32 * _balloonAnimationFrame, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 15.0f, SpriteEffects.None, 0);
            if (IsFloating)
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.5 && _stringForward)
                {
                    _stringAnimationFrame++;
                    _animationTimer = 0;
                    if (_stringAnimationFrame == 5) _stringForward = false;
                }
                else if (_animationTimer > 0.5 && !_stringForward)
                {
                    _stringAnimationFrame--;
                    _animationTimer = 0;
                    if (_stringAnimationFrame == 0) _stringForward = true;
                }
                spriteBatch.Draw(_stringTexture, Position, new Rectangle(32 * _stringAnimationFrame, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 15.0f, SpriteEffects.None, 0);
            }
        }
    }
}
