using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using qvp.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using qvp.Players;
using Microsoft.Xna.Framework.Audio;
using System.Reflection.Metadata;

namespace qvp.Screens
{
    public class StandoffGameScreen : GameScreen
    {
        private ContentManager _content;

        private CowboyPlayer _playerQ;
        private CowboyPlayer _playerP;

        private Texture2D _background;
        private Texture2D _text;

        InputAction _pause;
        InputAction _q;
        InputAction _p;

        private double _drawTimer;
        private bool _draw;

        private double _transitionTimer = 3;

        public override void Activate()
        {
            _pause = new InputAction(
                new[] { Keys.Escape }, true);
            _q = new InputAction(
                new[] { Keys.Q });
            _p = new InputAction(
                new[] { Keys.P });

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("draw_bg");
            _text = _content.Load<Texture2D>("draw_text");
            _playerQ = new CowboyPlayer(new Vector2(300, 800), _content.Load<Texture2D>("q_cowboy_idle"), _content.Load<Texture2D>("q_cowboy_shoot"), _content.Load<Texture2D>("q_cowboy_explode"), _content.Load<Texture2D>("p_dart"), _content.Load<SoundEffect>("explosion"), _content.Load<SoundEffect>("explosion2"));
            _playerP = new CowboyPlayer(new Vector2(975, 800), _content.Load<Texture2D>("p_cowboy_idle"), _content.Load<Texture2D>("p_cowboy_shoot"), _content.Load<Texture2D>("p_cowboy_explosion"), _content.Load<Texture2D>("q_dart"), _content.Load<SoundEffect>("explosion"), _content.Load<SoundEffect>("explosion2"));

            Random random = new Random();
            _drawTimer = random.Next(3, 6);
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
            if (_playerQ.IsSploded || _playerP.IsSploded || _playerQ.IsDead || _playerP.IsDead)
            {
                _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_transitionTimer < 0)
            {
                if (_playerQ.IsSploded || _playerQ.IsDead) ScreenManager.PPoints++;
                else ScreenManager.QPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }
            if (!_playerQ.IsDead && _q.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerQ.IsShooting = true;
            }
            if (!_playerP.IsDead && _p.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerP.IsShooting = true;
            }
            if (_playerQ.FinishedShooting)
            {
                _playerP.IsDead = true;
                _playerP.IsShooting = false;
            }
            if (_playerP.FinishedShooting)
            {
                _playerQ.IsDead = true;
                _playerQ.IsShooting = false;
            }
            if (_drawTimer > 0 && _playerQ.IsShooting && !_playerP.IsSploded)
            {
                _playerQ.IsSploded = true;
            }
            if (_drawTimer > 0 && _playerP.IsShooting && !_playerQ.IsSploded)
            {
                _playerP.IsSploded = true;
            }
            if (_drawTimer < 0)
            {
                _draw = true;
            }
            _drawTimer -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            if (_draw && !_playerQ.IsSploded && !_playerP.IsSploded) spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            _playerQ.Draw(spriteBatch, gameTime);
            _playerP.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
