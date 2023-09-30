using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class HandPlayer
    {
        private Vector2 _position;
        private Texture2D _texture;
        private float _velocity;
        public bool IsReady;

        public HandPlayer(Vector2 Position, Texture2D texture, float velocity)
        {
            _position = Position;
            _texture = texture;
            _velocity = velocity;
        }

        public void Update()
        {
            if (_position.X != 960)
            {
                _position.X += _velocity;
            }
            else if (_position.X == 960)
            {
                IsReady = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
        }
    }
}
