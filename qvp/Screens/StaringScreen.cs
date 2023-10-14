using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using qvp.Players;
using qvp.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace qvp.Screens
{
    public class StaringScreen : GameScreen
    {
        private ContentManager _content;

        private InputAction _pause;
        private InputAction _q;
        private InputAction _p;

        private StarePlayer _playerQ;
        private StarePlayer _playerP;

        private Texture2D _background;
        private Texture2D _text;

        private double _transitionTimer = 3;

        public override void Activate()
        {
            _pause = new InputAction(
                new[] { Keys.Escape }, true);
            _q = new InputAction(
                new[] { Keys.Q }, false);
            _p = new InputAction(
                new[] { Keys.P }, false);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("bg_mainmenu");
            _text = _content.Load<Texture2D>("stare_text");

            _playerQ = new StarePlayer(_content.Load<Texture2D>("q_stare"));
            _playerP = new StarePlayer(_content.Load<Texture2D>("p_stare"));
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

            if (_playerQ.IsDead || _playerP.IsDead)
            {
                _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_transitionTimer < 0)
            {
                if (_playerQ.IsDead) ScreenManager.PPoints++;
                else ScreenManager.QPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }

            if (_pause.Occurred(input, ControllingPlayer, out playerIndex))
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (ESC) to resume\npress (SPACE) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }

            if (_q.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerQ.Start = true;
                _playerQ.IsStaring = true;
            }
            else
            {
                _playerQ.IsStaring = false;
                if (_playerP.Start && _playerQ.Start && !_playerP.IsDead)
                {
                    _playerQ.IsDead = true;
                }
            }

            if (_p.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _playerP.Start = true;
                _playerP.IsStaring = true;
            }
            else
            {
                _playerP.IsStaring = false;
                if (_playerP.Start && _playerQ.Start && !_playerQ.IsDead)
                {
                    _playerP.IsDead = true;
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            _playerQ.Draw(spriteBatch, gameTime);
            _playerP.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
