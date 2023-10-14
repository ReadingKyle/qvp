using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qvp.Players
{
    public class BoxerPlayer
    {
        private Texture2D [] _textures;
        public Vector2 Position;
        public bool IsPunching;
        public bool FrontHand;
        public bool IsPunched;
        public int PunchesTaken;
        public bool IsDead;
        private int _animationFrame;

        public BoxerPlayer(Texture2D bodyTexture, Texture2D backArmTexture, Texture2D frontArmTexture, Vector2 position)
        {
            _textures = new Texture2D[3] { bodyTexture, backArmTexture, frontArmTexture };
            Position = position;
            PunchesTaken = 0;
            FrontHand = true;
        }

        public void Update()
        {
            if (IsPunched)
            {
                PunchesTaken++;
            }
            if (PunchesTaken >= 30)
            {
                IsDead = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, int bodyPartIndex)
        {
            if (bodyPartIndex == 1 && IsPunching && FrontHand) _animationFrame = 1;
            else if (bodyPartIndex == 1) _animationFrame = 0;

            if (bodyPartIndex == 2 && IsPunching && !FrontHand) _animationFrame = 1;
            else if (bodyPartIndex == 2) _animationFrame = 0;

            if (bodyPartIndex == 0 && IsDead) _animationFrame = 1;
            else if (bodyPartIndex == 0) _animationFrame = 0;

            spriteBatch.Draw(_textures[bodyPartIndex], Position, new Rectangle(_animationFrame * 32, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 15.0f, SpriteEffects.None, 0);
        }
    }
}
