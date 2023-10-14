using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class StarePlayer
    {
        private Texture2D _texture;
        private int _animationFrame;
        public bool IsStaring;
        public bool Start;
        public bool IsDead;

        public StarePlayer(Texture2D texture)
        {
            _texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsStaring && !IsDead) _animationFrame = 1;
            else _animationFrame = 0;
            spriteBatch.Draw(_texture, new Vector2(960, 960), new Rectangle(_animationFrame * 64, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
        }
    }
}
