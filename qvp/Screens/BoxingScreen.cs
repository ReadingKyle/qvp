using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using qvp.StateManagement;
using qvp.Players;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.TimeZoneInfo;
using Microsoft.Xna.Framework.Audio;

namespace qvp.Screens
{
    public class BoxingScreen : GameScreen
    {
        private ContentManager _content;

        private InputAction _pause;
        private InputAction _q;
        private InputAction _p;

        private Texture2D _background;
        private Texture2D _foreground;

        SoundEffectInstance _qPunchSound;
        SoundEffectInstance _pPunchSound;
        SoundEffectInstance _koSound;

        private BoxerPlayer _playerQ;
        private BoxerPlayer _playerP;

        //private Debugger _debugger;
        private double _transitionTimer = 3;

        private bool _playKOSound;

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

            _background = _content.Load<Texture2D>("ring_background");
            _foreground = _content.Load<Texture2D>("ring_foreground");

            _playerQ = new BoxerPlayer(_content.Load<Texture2D>("q_boxer_body"), _content.Load<Texture2D>("q_boxer_frontarm"), _content.Load<Texture2D>("q_boxer_backarm"), new Vector2(642,645));
            _playerP = new BoxerPlayer(_content.Load<Texture2D>("p_boxer_body"), _content.Load<Texture2D>("p_boxer_frontarm"), _content.Load<Texture2D>("p_boxer_backarm"), new Vector2(776, 645));

            SoundEffect qPunchSound = _content.Load<SoundEffect>("jump");
            SoundEffect pPunchSound = _content.Load<SoundEffect>("jump (4)");
            SoundEffect KOSound = _content.Load<SoundEffect>("pickupCoin");
            _koSound = KOSound.CreateInstance();
            _qPunchSound = qPunchSound.CreateInstance();
            _pPunchSound = pPunchSound.CreateInstance();
            _qPunchSound.Volume = 0.5f;
            _pPunchSound.Volume = 0.5f;
            _koSound.Volume = 0.5f;
            _playKOSound = true;

            //_debugger = new Debugger(_playerQ);
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
            PlayerIndex playerIndex = 0;

            //_debugger.Update(gameTime, input, playerIndex, playerIndex);

            if (_pause.Occurred(input, ControllingPlayer, out playerIndex))
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (ESC) to resume\npress (SPACE) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }

            if (_playerQ.IsDead || _playerP.IsDead)
            {
                if (_playKOSound)
                {
                    _koSound.Play();
                    _playKOSound = false;
                }
                _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_transitionTimer < 0)
            {
                if (_playerQ.IsDead) ScreenManager.PPoints++;
                else ScreenManager.QPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }
            if (_q.Occurred(input, ControllingPlayer, out playerIndex) && !_playerQ.IsDead)
            {
                _qPunchSound.Play();
                _playerP.IsPunched = true;
                _playerP.Update();
                _playerQ.IsPunching = true;
                _playerQ.FrontHand = !_playerQ.FrontHand;
            }
            else
            {
                _playerP.IsPunched = false;
            }
            if (_p.Occurred(input, ControllingPlayer, out playerIndex) && !_playerP.IsDead)
            {
                _pPunchSound.Play();
                _playerQ.IsPunched = true;
                _playerQ.Update();
                _playerP.IsPunching = true;
                _playerP.FrontHand = !_playerP.FrontHand;
            }
            else
            {
                _playerQ.IsPunched = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            _playerQ.Draw(spriteBatch, gameTime, 2);
            _playerP.Draw(spriteBatch, gameTime, 2);
            _playerQ.Draw(spriteBatch, gameTime, 0);
            _playerP.Draw(spriteBatch, gameTime, 0);
            _playerQ.Draw(spriteBatch, gameTime, 1);
            _playerP.Draw(spriteBatch, gameTime, 1);
            spriteBatch.Draw(_foreground, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
