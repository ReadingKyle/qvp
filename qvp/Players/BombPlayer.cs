using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class BombPlayer
    {
        private Texture2D _texture;
        public Vector2 Position;
        private int _animationFrame = 0;
        private double _animationTimer;
        public bool IsThrowing;
        public bool IsHolding;
        public bool IsCatching;
        public bool IsResting;
        public bool IsSploded;

        public BombPlayer(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsThrowing)
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.05)
                {
                    _animationFrame++;
                    _animationTimer -= 0.1;
                }
                if (_animationFrame == 2)
                {
                    IsThrowing = false;
                    IsResting = true;
                }
            }
            if (IsCatching)
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.05)
                {
                    _animationFrame--;
                    _animationTimer -= 0.1;
                }
                if (_animationFrame == 0)
                {
                    IsCatching = false;
                    IsHolding = true;
                }
            }
            if (IsResting)
            {
                _animationFrame = 2;
            }
            if (IsHolding)
            {
                _animationFrame = 0;
            }
            if (IsSploded)
            {
                _animationFrame = 3;
            }
            spriteBatch.Draw(_texture, Position, new Rectangle(32 * _animationFrame, 0, 32, 32), Color.White, 0, new Vector2(0, 0), 15.0f, SpriteEffects.None, 0);
        }
    }
}
