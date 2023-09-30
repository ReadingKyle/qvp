using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace qvp.StateManagement
{
    public class Laser
    {
        private Vector2 _position;
        private float _velocity;

        private Texture2D _texture;

        private BoundingRectangle _bounds;

        public BoundingRectangle Bounds => _bounds;

        public Laser(Texture2D laserTexture, Vector2 initPosition, float velocity)
        { 
            _position = initPosition;
            _velocity = velocity;
            _texture = laserTexture;
            _bounds = new BoundingRectangle(_position, 1, 1);
        }

        public void Update()
        {
            _position.X += _velocity;
            _bounds.X = _position.X;
            _bounds.Y = _position.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle hitBox = Bounds.ToRectangle();
            spriteBatch.Draw(_texture, new Vector2(hitBox.X, hitBox.Y), hitBox, Color.White, 0, new Vector2(0, 0), 10.0f, SpriteEffects.None, 0);
        }
    }
}
