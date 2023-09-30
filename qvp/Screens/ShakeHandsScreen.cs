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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace qvp.Screens
{
    public class ShakeHandsScreen : GameScreen
    {
        private ContentManager _content;
        private Texture2D _background;
        private Texture2D _handShake;
        private Texture2D _text;
        InputAction _q;
        InputAction _p;

        private HandPlayer _playerQ;
        private HandPlayer _playerP;

        private double animationTimer;
        private int animationFrame;

        private double _shakeTimer = 1;
        private int _shakePosition;

        private SoundEffect _handShakeUp;
        private SoundEffect _handShakeDown;

        private bool _handShakeState = true;

        public override void Activate()
        {
            _q = new InputAction(
                new[] { Keys.Q }, false);
            _p = new InputAction(
                new[] { Keys.P }, false);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("handshake_bg");
            _text = _content.Load<Texture2D>("handshake_text");
            _handShake = _content.Load<Texture2D>("handshake");
            _playerQ = new HandPlayer(new Vector2(350, 960), _content.Load<Texture2D>("q_handshake"), 5);
            _playerP = new HandPlayer(new Vector2(1550, 960), _content.Load<Texture2D>("p_handshake"), -5);
            _handShakeUp = _content.Load<SoundEffect>("jump (3)");
            _handShakeDown = _content.Load<SoundEffect>("jump (7)");
            MediaPlayer.Pause();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (!_playerQ.IsReady || !_playerP.IsReady)
            {
                if (_q.Occurred(input, ControllingPlayer, out playerIndex))
                {
                    _playerQ.Update();
                }
                if (_p.Occurred(input, ControllingPlayer, out playerIndex))
                {
                    _playerP.Update();
                }
            }
            else
            {
                _shakeTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_shakeTimer < 1 && _shakeTimer > 0.75)
                {
                    if (_handShakeState == true) _handShakeUp.Play();
                    _handShakeState = false;
                    _shakePosition = -100;
                }
                else if (_shakeTimer < 0.75 && _shakeTimer > 0.5)
                {
                    if (_handShakeState == false) _handShakeDown.Play();
                    _handShakeState = true;
                    _shakePosition = 100;
                }
                else if (_shakeTimer < 0.5 && _shakeTimer > 0.25)
                {
                    if (_handShakeState == true) _handShakeUp.Play();
                    _handShakeState = false;
                    _shakePosition = -100;
                }
                else if (_shakeTimer < 0.25 && _shakeTimer > 0)
                {
                    if (_handShakeState == false) _handShakeDown.Play();
                    _handShakeState = true;
                    _shakePosition = 100;
                }
                if (_shakeTimer < 0)
                {
                    MediaPlayer.Resume();
                    LoadingScreen.Load(ScreenManager, false, new MenuScreen());
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 0.5)
            {
                animationFrame++;
                if (animationFrame > 1) animationFrame = 0;
                animationTimer -= 0.5;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(animationFrame * 64, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            if (!_playerQ.IsReady || !_playerP.IsReady)
            {
                _playerP.Draw(spriteBatch);
                _playerQ.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.Draw(_handShake, new Vector2(960, 960 + _shakePosition), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }
    }
}
