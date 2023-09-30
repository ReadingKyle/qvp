using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class CowboyPlayer
    {
        private Vector2 _position;
        private Texture2D _idleTexture;
        private Texture2D _shootTexture;
        private Texture2D _explodeTexture;
        private Texture2D _dart;
        public bool IsShooting = false;
        public bool IsDead = false;
        public bool FinishedShooting = false;
        public bool IsSploded = false;

        private double idleAnimationTimer;
        private short idleAnimationFrame;
        
        private double shootAnimationTimer;
        private short shootAnimationFrame;

        private double explosionAnimationTimer;
        private short explosionAnimationFrame;

        private SoundEffect _shootPlayer;
        private SoundEffect _shootGround;

        public CowboyPlayer(Vector2 Position, Texture2D idleTexture, Texture2D shootTexture, Texture2D explodeTexture, Texture2D dart, SoundEffect shootPlayer, SoundEffect shootGround)
        {
            _position = Position;
            _idleTexture = idleTexture;
            _shootTexture = shootTexture;
            _explodeTexture = explodeTexture;
            _dart = dart;
            _shootPlayer = shootPlayer;
            _shootGround = shootGround;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsSploded)
            {
                explosionAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (explosionAnimationTimer > 0.05)
                {
                    explosionAnimationFrame++;
                    if (explosionAnimationFrame == 3)
                    {
                        _shootGround.Play();
                    }
                    if (explosionAnimationFrame > 11)
                    {
                        explosionAnimationFrame = 11;
                    }
                    explosionAnimationTimer -= 0.05;
                }
                spriteBatch.Draw(_explodeTexture, _position, new Rectangle(explosionAnimationFrame * 32, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 10.0f, SpriteEffects.None, 0);
            }
            else if (IsShooting)
            {
                shootAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (shootAnimationTimer > 0.05 && !IsDead)
                {
                    shootAnimationFrame++;
                    if (shootAnimationFrame == 4)
                    {
                        _shootPlayer.Play();
                    }
                    if (shootAnimationFrame > 8 && !IsDead)
                    {
                        shootAnimationFrame = 8;
                        FinishedShooting = true;
                    }
                    shootAnimationTimer -= 0.05;
                }
                spriteBatch.Draw(_shootTexture, _position, new Rectangle(shootAnimationFrame * 32, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 10.0f, SpriteEffects.None, 0);
            }
            else
            {
                idleAnimationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (idleAnimationTimer > 0.5 && !IsDead)
                {
                    idleAnimationFrame++;
                    if (idleAnimationFrame > 3) idleAnimationFrame = 0;
                    idleAnimationTimer -= 0.5;
                }
                spriteBatch.Draw(_idleTexture, _position, new Rectangle(idleAnimationFrame * 32, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 10.0f, SpriteEffects.None, 0);
            }
            if (IsDead && !IsSploded)
            {
                float dartoffset = 0;
                if (_position.X < 600) dartoffset = -100;
                else dartoffset = -180;
                spriteBatch.Draw(_dart, new Vector2(_position.X + dartoffset, _position.Y - 200), new Rectangle(0, 0, 3, 1), Color.White, 0, new Vector2(3, 1), 10.0f, SpriteEffects.None, 0);
            }
        }
    }
}
