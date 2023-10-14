using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using qvp.Players;
using qvp.StateManagement;
using SharpDX.Direct2D1;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.TimeZoneInfo;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace qvp.Screens
{
    public class BalloonGameScreen : GameScreen
    {
        private ContentManager _content;

        private InputAction _pause;
        private InputAction _qPump;
        private InputAction _pPump;
        private InputAction _q;
        private InputAction _p;

        private Texture2D _background;
        private Texture2D _text;
        private Texture2D _clouds;
        private Texture2D _balloonStand;

        SoundEffectInstance _qPumpSound;
        SoundEffectInstance _pPumpSound;

        private BalloonPlayer _balloonQ;
        private BalloonPlayer _balloonP;

        private PumpPlayer _playerQ;
        private PumpPlayer _playerP;

        //private Debugger _debugger;

        private SpriteFont _font;

        private double _countDown;

        private double _screenPosition;
        private double _transitionTimer = 3;

        private int _animationFrame;
        private double _animationTimer;
        private bool _cloudsForward;

        public override void Activate()
        {
            _pause = new InputAction(
                new[] { Keys.Escape }, true);
            _qPump = new InputAction(
                new[] { Keys.Q }, false);
            _pPump = new InputAction(
                new[] { Keys.P }, false);
            _q= new InputAction(
                new[] { Keys.Q }, true);
            _p= new InputAction(
                new[] { Keys.P }, true);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("balloon_bluesky");
            _text = _content.Load<Texture2D>("balloon_text");
            _clouds = _content.Load<Texture2D>("balloon_sky_clouds");
            _balloonStand = _content.Load<Texture2D>("balloon_stand");

            SoundEffect qPumpSound = _content.Load<SoundEffect>("jump");
            SoundEffect pPumpSound = _content.Load<SoundEffect>("jump (4)");
            _qPumpSound = qPumpSound.CreateInstance();
            _pPumpSound = pPumpSound.CreateInstance();
            _qPumpSound.Volume = 0.5f;
            _pPumpSound.Volume = 0.5f;

            _balloonQ = new BalloonPlayer(_content.Load<Texture2D>("q_balloon"), _content.Load<Texture2D>("balloon_string"), new Vector2(414, 960));
            _balloonP = new BalloonPlayer(_content.Load<Texture2D>("p_balloon"), _content.Load<Texture2D>("balloon_string"), new Vector2(1020, 960));

            _playerQ = new PumpPlayer(_content.Load<Texture2D>("q_pump"), new Vector2(-66, 450));
            _playerP = new PumpPlayer(_content.Load<Texture2D>("p_pump"), new Vector2(708, 450));

            _font = ScreenManager.Font;
            _countDown = 10;

            //_debugger = new Debugger(_balloonP);
            _screenPosition = 0;
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

            if (_balloonQ.IsFinished && _balloonP.IsFinished)
            {
                _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_transitionTimer < 0)
            {
                if (_balloonQ.NumPumps >= _balloonP.NumPumps) ScreenManager.QPoints++;
                else ScreenManager.PPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }

            if (_countDown > 0)
            {
                _countDown -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_countDown < 0)
            {
                _balloonQ.IsFloating = true;
                _balloonP.IsFloating = true;
            }
            if (_pause.Occurred(input, ControllingPlayer, out playerIndex))
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (ESC) to resume\npress (SPACE) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }

            if (_q.Occurred(input, ControllingPlayer, out playerIndex) && _countDown > 0)
            {
                _qPumpSound.Play();
                _balloonQ.NumPumps++;
            }
            if (_qPump.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerQ.IsPumped = true;
            }
            else
            {
                _playerQ.IsPumped = false;
            }

            if (_p.Occurred(input, ControllingPlayer, out playerIndex) && _countDown > 0)
            {
                _pPumpSound.Play();
                _balloonP.NumPumps++;
            }
            if (_pPump.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerP.IsPumped = true;
            }
            else
            {
                _playerP.IsPumped = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            Matrix transform;

            if (_countDown < 0 && (!_balloonQ.IsFinished || !_balloonP.IsFinished))
            {
                _screenPosition += 1500 * gameTime.ElapsedGameTime.TotalSeconds;
            }
            transform = Matrix.CreateTranslation(0, (float)_screenPosition * 0.333f, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, transformMatrix: transform);
            spriteBatch.Draw(_background, new Vector2(960, 960 * -18), new Rectangle(0, 0, 64, 1280), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_balloonStand, new Vector2(-66, 480), new Rectangle(0, 0, 32, 32), Color.White, 0, new Vector2(0, 0), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_balloonStand, new Vector2(540, 480), new Rectangle(0, 0, 32, 32), Color.White, 0, new Vector2(0, 0), 15.0f, SpriteEffects.None, 0);


            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_animationTimer > 0.5)
            {
                _animationFrame++;
                _animationTimer = 0;
                if (_animationFrame == 5) _animationFrame = 0;
            }
            spriteBatch.Draw(_clouds, new Vector2(960, 960 * -18), new Rectangle(64 * _animationFrame, 0, 64, 1280), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);

            if (_countDown > 0) spriteBatch.DrawString(_font, _countDown.ToString("F0"), new Vector2(425, 500), Color.Red, 0, new Vector2(0, 0), 5f, SpriteEffects.None, 0);

            for (int i = 0; i < 1800; i+=5)
            {
                spriteBatch.DrawString(_font, (i+5).ToString(), new Vector2(900, i*-101.90f - 200), Color.Red, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }

            _playerQ.Draw(spriteBatch, gameTime);
            _playerP.Draw(spriteBatch, gameTime);
            _balloonQ.Draw(spriteBatch, gameTime);
            _balloonP.Draw(spriteBatch, gameTime);

            spriteBatch.End();
        }
    }
}
