using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using qvp.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class SpaceShipPlayer
    {
        private Vector2 _position;
        private Texture2D _shipTexture;
        private Texture2D _laserTexture;
        public bool IsPressing;
        public Laser Laser;
        private float _moveVelocity;
        private float _laserVelocity;
        private double _shotCooldown;
        private bool _facingRight;
        public bool IsSploded = false;

        private double explosionAnimationTimer;
        private int explosionAnimationFrame;

        private BoundingRectangle _bounds;
        public BoundingRectangle Bounds => _bounds;

        private SoundEffect _laserShoot;
        private SoundEffect _explode;

        public SpaceShipPlayer(Vector2 Position, Texture2D texture, Texture2D laserTexture, float moveVelocity, float laserVelocity, bool facingRight, SoundEffect laserShoot, SoundEffect explode)
        {
            _position = Position;
            _shipTexture = texture;
            _moveVelocity = moveVelocity;
            _laserVelocity = laserVelocity;
            _laserTexture = laserTexture;
            _facingRight = facingRight;
            _bounds = new BoundingRectangle(_position, 100, 75);
            _laserShoot = laserShoot;
            _explode = explode;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsSploded)
            {
                _bounds.X = _position.X - 130;
                _bounds.Y = _position.Y - 125;
                _shotCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_position.Y <= 125 || _position.Y >= 1000)
                {
                    Random random = new Random();
                    _moveVelocity *= random.Next(1, 2);
                    _moveVelocity *= -1;
                }
                _position.Y += _moveVelocity;
                if (Laser != null)
                {
                    Laser.Update();
                }
            }
        }

        public void Shoot()
        {
            if (!IsSploded && _shotCooldown < 0)
            {
                _laserShoot.Play();
                int laserXOffset = _facingRight ? -30 : -85;
                int laserYOffset = -45;
                Laser = new Laser(_laserTexture, new Vector2(_position.X + laserXOffset, _position.Y + laserYOffset), _laserVelocity);
                _shotCooldown = 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Laser != null)
            {
                Laser.Draw(spriteBatch);
            }
            if (IsSploded)
            {
                explosionAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (explosionAnimationTimer > 0.05)
                {
                    explosionAnimationFrame++;
                    if (explosionAnimationFrame == 1)
                    {
                        _explode.Play();
                    }
                    if (explosionAnimationFrame > 6)
                    {
                        explosionAnimationFrame = 6;
                    }
                    explosionAnimationTimer -= 0.05;
                }
            }
            spriteBatch.Draw(_shipTexture, _position, new Rectangle(explosionAnimationFrame * 32, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 5.0f, SpriteEffects.None, 0);
            
            // Debug
            //Rectangle hitBox = Bounds.ToRectangle();
            //spriteBatch.Draw(_laserTexture, new Vector2(hitBox.X, hitBox.Y), hitBox, Color.White, 0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
        }
    }
}
