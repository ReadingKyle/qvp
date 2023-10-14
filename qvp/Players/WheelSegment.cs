using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class WheelSegment
    {
        private Texture2D _texture;
        public Vector2 Position;
        public bool IsHit;

        public WheelSegment(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime elapsedGameTime, float wheelRotation)
        {
            spriteBatch.Draw(_texture, Position, new Rectangle(IsHit ? 32 : 0, 0, 32, 32), Color.White, wheelRotation, new Vector2(16, 16), 15.0f, SpriteEffects.None, 0);
        }
    }
}
