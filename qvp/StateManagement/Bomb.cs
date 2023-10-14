using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.StateManagement
{
    public class Bomb
    {
        private Texture2D _texture;
        public Vector2 Position;
        public Vector2 Velocity;
        public float _acceleration;
        public bool Throw;

        private int _animationFrame;
        private double _animationTimer;

        public bool IsSploded;

        public Bomb(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
            _acceleration = 3000f;
        }

        public void Update (GameTime gameTime)
        {
            if (Throw)
            {
                Position.X += Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position.Y += Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Velocity.Y += _acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds; ;
                if (Position.Y >= 480)
                {
                    Throw = false;
                }
            }
        }

        public void Draw (SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsSploded)
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 2 && _animationFrame != 6)
                {
                    _animationFrame++;
                    _animationTimer -= 2;
                }
                spriteBatch.Draw(_texture, Position, new Rectangle(16 * _animationFrame, 0, 16, 16), Color.White, 0, new Vector2(0, 0), 15.0f, SpriteEffects.None, 0);
            }
        }
    }
}
