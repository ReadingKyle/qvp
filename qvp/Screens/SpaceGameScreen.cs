using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using qvp.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using qvp.Players;
using static System.TimeZoneInfo;
using Microsoft.Xna.Framework.Audio;

namespace qvp.Screens
{
    public class SpaceGameScreen : GameScreen
    {
        private ContentManager _content;

        private Texture2D _background;
        private Texture2D _text;

        private SpaceShipPlayer _shipQ;
        private SpaceShipPlayer _shipP;

        InputAction _q;
        InputAction _pause;
        InputAction _p;

        private double _transitionTimer = 3;

        public override void Activate()
        {
            _pause = new InputAction(
                new[] { Keys.Escape }, true);
            _q = new InputAction(
                new[] { Keys.Q }, true);
            _p = new InputAction(
                new[] { Keys.P }, true);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("space_bg");
            _text = _content.Load<Texture2D>("space_text");

            Random random = new Random();

            _shipQ = new SpaceShipPlayer(new Vector2(160, random.Next(165, 995)), _content.Load<Texture2D>("q_spaceship"), _content.Load<Texture2D>("q_spaceship_blast"), -10, 20, true, _content.Load<SoundEffect>("laserShoot"), _content.Load<SoundEffect>("explosion (2)"));
            _shipP = new SpaceShipPlayer(new Vector2(960, random.Next(165, 995)), _content.Load<Texture2D>("p_spaceship"), _content.Load<Texture2D>("p_spaceship_blast"), 10, -20, false, _content.Load<SoundEffect>("laserShoot"), _content.Load<SoundEffect>("explosion (2)"));
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, new MenuScreen());
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (_pause.Occurred(input, ControllingPlayer, out playerIndex))
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (ESC) to resume\npress (SPACE) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }
            if (_shipQ.IsSploded || _shipP.IsSploded)
            {
                _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_shipP.Laser != null && CollisionHelper.Collides(_shipQ.Bounds, _shipP.Laser.Bounds))
            {
                _shipQ.IsSploded = true;
            }
            if (_shipQ.Laser != null && CollisionHelper.Collides(_shipP.Bounds, _shipQ.Laser.Bounds))
            {
                _shipP.IsSploded = true;
            }

            if (_transitionTimer < 0)
            {
                if (_shipQ.IsSploded) ScreenManager.PPoints++;
                else ScreenManager.QPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }
            if (_q.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _shipQ.Shoot();
            }
            if (_p.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _shipP.Shoot();
            }
            _shipQ.Update(gameTime);
            _shipP.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            _shipQ.Draw(spriteBatch, gameTime);
            _shipP.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
