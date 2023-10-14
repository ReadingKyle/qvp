using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleSystemExample;
using qvp.Players;
using qvp.StateManagement;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace qvp.Screens
{
    public class BombGameScreen : GameScreen
    {
        private ContentManager _content;

        private InputAction _pause;
        private InputAction _q;
        private InputAction _p;

        private Texture2D _background;
        private Texture2D _text;

        SoundEffectInstance _qThrowSound;
        SoundEffectInstance _pThrowSound;
        SoundEffectInstance _explosionSound;

        private BombPlayer _playerQ;
        private BombPlayer _playerP;

        private Bomb _bomb;
        //private Debugger _debugger;

        private double _bombTimer;

        private ExplosionParticleSystem _explosions;

        private double _throwCooldown;
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

            _explosions = new ExplosionParticleSystem(ScreenManager.Game, 100);
            ScreenManager.Game.Components.Add(_explosions);

            _background = _content.Load<Texture2D>("bg_mainmenu");
            _text = _content.Load<Texture2D>("bomb_text");

            _playerQ = new BombPlayer(_content.Load<Texture2D>("q_bomb"), new Vector2(-30, 480));
            _playerP = new BombPlayer(_content.Load<Texture2D>("p_bomb"), new Vector2(500, 480));

            //_debugger = new Debugger(_bomb);

            Random random = new Random();
            if (random.Next(1, 3) == 1)
            {
                _playerQ.IsHolding = true;
                _playerP.IsResting = true;
                _bomb = new Bomb(_content.Load<Texture2D>("bomb"), new Vector2(15, 480));
            }
            else
            {
                _playerP.IsHolding = true;
                _playerQ.IsResting = true;
                _bomb = new Bomb(_content.Load<Texture2D>("bomb"), new Vector2(698, 480));
            }

            _bombTimer = random.Next(5, 15);
            _throwCooldown = 0;

            SoundEffect qThrowSound = _content.Load<SoundEffect>("jump");
            SoundEffect pThrowSound = _content.Load<SoundEffect>("jump (4)");
            SoundEffect explosionSound = _content.Load<SoundEffect>("explosion (2)");
            _explosionSound = explosionSound.CreateInstance();
            _qThrowSound = qThrowSound.CreateInstance();
            _pThrowSound = pThrowSound.CreateInstance();
            _qThrowSound.Volume = 0.5f;
            _pThrowSound.Volume = 0.5f;
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

            _bombTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            _throwCooldown -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_bomb.IsSploded)
            {
                _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_transitionTimer < 0)
            {
                if (_playerQ.IsSploded) ScreenManager.PPoints++;
                else ScreenManager.QPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }

            if (_bombTimer < 0)
            {
                if (_playerQ.IsHolding)
                {
                    _explosionSound.Play();
                    _playerQ.IsSploded = true;
                    _playerQ.IsHolding = false;
                    _explosions.PlaceExplosion(new Vector2(_bomb.Position.X + 24, _bomb.Position.Y + 24));
                }
                else if (_playerP.IsHolding)
                {
                    _explosionSound.Play();
                    _playerP.IsHolding = false;
                    _playerP.IsSploded = true;
                    _explosions.PlaceExplosion(new Vector2(_bomb.Position.X + 24, _bomb.Position.Y + 24));
                }
                _bomb.IsSploded = true;
            }

            if (_pause.Occurred(input, ControllingPlayer, out playerIndex))
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (ESC) to resume\npress (SPACE) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }
            if (_q.Occurred(input, ControllingPlayer, out playerIndex) && _playerQ.IsHolding && !_playerQ.IsSploded && _throwCooldown <= 0 && _bombTimer - 1 >= 0)
            {
                _qThrowSound.Play();
                _playerQ.IsThrowing = true;
                _playerQ.IsHolding = false;
                _playerP.IsCatching = true;
                _playerP.IsResting = false;
                _bomb.Throw = true;
                _bomb.Velocity = new Vector2(1000, -1000);
                _throwCooldown = 0.75;
            }
            if (_p.Occurred(input, ControllingPlayer, out playerIndex) && _playerP.IsHolding && !_playerP.IsSploded && _throwCooldown <= 0 && _bombTimer - 1 >= 0)
            {
                _pThrowSound.Play();
                _playerP.IsThrowing = true;
                _playerP.IsHolding = false;
                _playerQ.IsCatching = true;
                _playerQ.IsResting = false;
                _bomb.Throw = true;
                _bomb.Velocity = new Vector2(-1000, -1000);
                _throwCooldown = 0.75;
            }
            _bomb.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);

            _playerQ.Draw(spriteBatch, gameTime);
            _playerP.Draw(spriteBatch, gameTime);
            _bomb.Draw(spriteBatch, gameTime);

            spriteBatch.End();
        }
    }
}
