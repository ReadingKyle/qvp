using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class RunnerPlayer
    {
        private Vector2 _position;
        private Texture2D _textureRunning;
        private Texture2D _textureIdle;
        private double animationTimer;
        private int animationFrame;
        public bool IsRunning;
        public double RunCooldown = 0.25;
        public double Velocity = 1;
        public bool Restart;
        public bool HasWon;

        public RunnerPlayer(Vector2 Position, Texture2D textureRunning, Texture2D textureIdle)
        {
            _position = Position;
            _textureRunning = textureRunning;
            _textureIdle = textureIdle;
        }

        public void Update (GameTime gameTime)
        {
            if (Restart && _position.Y > 100)
            {
                _position.Y -= 10;
            }
            else if (Restart && _position.Y <= 100)
            {
                Restart = false;
            }
            else if (IsRunning && RunCooldown > 0)
            {
                RunCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                _position.Y += (float)Velocity;
            }
            else
            {
                IsRunning = false;
                RunCooldown = 0.25;
                Velocity = 1;
            }
            if (_position.Y > 960)
            {
                HasWon = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsRunning)
            {
                animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (animationTimer > 0.15 * Velocity)
                {
                    animationFrame++;
                    if (animationFrame > 3) animationFrame = 0;
                    animationTimer -= 0.15 / Velocity;
                }
                spriteBatch.Draw(_textureRunning, _position, new Rectangle(animationFrame * 32, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 5.0f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(_textureIdle, _position, new Rectangle(0, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 5.0f, SpriteEffects.None, 0);
            }
        }
    }
}
