using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace qvp.Players
{
    public class DartPlayer
    {
        private Texture2D _texture;
        private Vector2 _position;
        public bool IsStuck;
        public bool IsThrowing;
        public float Rotation;

        public int _animationFrame = 0;
        public double _animationTimer;

        public DartPlayer (Texture2D texture, Vector2 position)
        {
            _position = position;
            _texture = texture;
        }

        public void Update(float wheelRotation)
        {
            if (_animationFrame == 2)
            {
                IsStuck = true;
                Rotation = wheelRotation;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float wheelRotation)
        {
            if (IsThrowing && _animationFrame != 2)
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.05)
                {
                    _animationFrame++;
                    _animationTimer -= 0.1;
                }
                if (_animationFrame == 2)
                {
                    IsStuck = true;
                }
            }
            spriteBatch.Draw(_texture, _position, new Rectangle(32 * _animationFrame, 0, 32, 32), Color.White, IsStuck ? Rotation : 0, new Vector2(IsStuck ? 24 : 16, IsStuck ? 24 : 16), IsStuck ? 7f : 15.0f, SpriteEffects.None, 0);
        }
    }
}
