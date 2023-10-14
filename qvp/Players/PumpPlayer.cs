using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class PumpPlayer
    {
        private Texture2D _texture;
        public Vector2 Position;
        public bool IsPumped;

        public PumpPlayer(Texture2D texture, Vector2 position) 
        {
            _texture = texture;
            Position = position;
        }

        public void Draw (SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, Position, new Rectangle(IsPumped ? 32 : 0, 0, 32, 32), Color.White, 0, new Vector2(0, 0), 10.0f, SpriteEffects.None, 0);
        }
    }
}
