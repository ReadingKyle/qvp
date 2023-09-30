using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using qvp.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qvp.Players;
using Microsoft.Xna.Framework.Audio;

namespace qvp.Screens
{
    public class TrackGameScreen : GameScreen
    {
        private ContentManager _content;
        private Texture2D _background;
        private Texture2D _redLight;
        private Texture2D _greenLight;
        private Texture2D _text;

        InputAction _pause;
        InputAction _q;
        InputAction _p;

        private RunnerPlayer _playerQ;
        private RunnerPlayer _playerP;

        private double _lightChangeTimer;
        private bool IsRed;

        SoundEffectInstance _qRunSound;
        SoundEffectInstance _pRunSound;

        public override void Activate()
        {
            _pause = new InputAction(
                new[] { Keys.Escape }, true);
            _q = new InputAction(
                new[] { Keys.Q }, true);
            _p = new InputAction(
                new[] { Keys.P}, true);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("track_bg");
            _redLight = _content.Load<Texture2D>("track_redlight");
            _greenLight = _content.Load<Texture2D>("track_greenlight");
            _text = _content.Load<Texture2D>("track_text");
            _playerQ = new RunnerPlayer(new Vector2(452, 100), _content.Load<Texture2D>("q_runner_running"), _content.Load<Texture2D>("q_runner_idle"));
            _playerP = new RunnerPlayer(new Vector2(672, 100), _content.Load<Texture2D>("p_runner_running"), _content.Load<Texture2D>("p_runner_idle"));
            SoundEffect qRunSound = _content.Load<SoundEffect>("jump");
            SoundEffect pRunSound = _content.Load<SoundEffect>("jump (4)");

            _qRunSound = qRunSound.CreateInstance();
            _pRunSound = pRunSound.CreateInstance();
            _qRunSound.Volume = 0.5f;
            _pRunSound.Volume = 0.5f;

            Random random = new Random();
            _lightChangeTimer = random.Next(1, 3);
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
            if (_lightChangeTimer <= 0)
            {
                Random random = new Random();
                _lightChangeTimer = random.Next(1, 6);
                IsRed = !IsRed;
            }
            if (_playerQ.HasWon && _playerP.HasWon)
            {
                ScreenManager.QPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }
            else if (_playerP.HasWon)
            {
                ScreenManager.PPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }
            else if (_playerQ.HasWon)
            {
                ScreenManager.QPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }
            if (_q.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _qRunSound.Play();
                _playerQ.IsRunning = true;
                _playerQ.RunCooldown = 0.25;
                _playerQ.Velocity += 0.25;
            }
            if (_q.Occurred(input, ControllingPlayer, out playerIndex) && IsRed)
            {
                _playerQ.Restart = true;
            }
            if (_p.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _pRunSound.Play();
                _playerP.IsRunning = true;
                _playerP.RunCooldown = 0.25;
                _playerP.Velocity += 0.25;
            }
            if (_p.Occurred(input, ControllingPlayer, out playerIndex) && IsRed)
            {
                _playerP.Restart = true;
            }
            _playerQ.Update(gameTime);
            _playerP.Update(gameTime);

            _lightChangeTimer -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_redLight, new Vector2(960, 960), new Rectangle(IsRed ? 64 : 0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_greenLight, new Vector2(960, 960), new Rectangle(IsRed ? 0 : 64, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            _playerQ.Draw(spriteBatch, gameTime);
            _playerP.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
