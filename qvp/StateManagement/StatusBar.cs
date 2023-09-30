using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.StateManagement
{
    public class StatusBar
    {
        private Texture2D _texture;
        private SoundEffect _readyUpSound;
        private Vector2 _position = new Vector2(960, 1920);
        public bool IsFull;

        public StatusBar (Texture2D texture, SoundEffect readyUpSound)
        {
            _texture = texture;
            IsFull = false;
            _readyUpSound = readyUpSound;
        }

        public void Update (bool add)
        {
            if (add && _position.Y > 955)
            {
                _position.Y -= 20;
                if (_position.Y <= 960)
                {  
                    IsFull = true;
                }
                if (_position.Y == 960)
                {
                    _readyUpSound.Play();
                }
            }
            else if (!add && _position.Y < 1920)
            {
                _position.Y += 10;
                if (_position.Y > 960)
                {
                    IsFull = false;
                }
            }
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_texture, _position, new Rectangle(0, 0, 64, 68), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
