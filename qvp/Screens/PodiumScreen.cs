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
using Microsoft.Xna.Framework.Audio;

namespace qvp.Screens
{
    public class PodiumScreen : GameScreen
    {
        private ContentManager _content;
        private Texture2D _background;
        private Texture2D _podium;
        private Texture2D _qCele;
        private Texture2D _pCele;
        private Texture2D _qSad;
        private Texture2D _pSad;

        InputAction _q;
        InputAction _p;
        InputAction _qPlaySound;
        InputAction _pPlaySound;

        private bool _qEmote = false;
        private bool _pEmote = false;

        private double _transitionTimer = 5;

        SoundEffect _qEmoteSound;
        SoundEffect _pEmoteSound;

        public override void Activate()
        {
            _q = new InputAction(
                new[] { Keys.Q }, false);
            _p = new InputAction(
                new[] { Keys.P }, false);
            _qPlaySound = new InputAction(
                new[] { Keys.Q }, true);
            _pPlaySound = new InputAction(
                new[] { Keys.P }, true);

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("bg_mainmenu");
            _podium = _content.Load<Texture2D>("podium_podiums");
            _qCele = _content.Load<Texture2D>("q-cele");
            _pCele = _content.Load<Texture2D>("p_cele");
            _qSad = _content.Load<Texture2D>("q_sad");
            _pSad = _content.Load<Texture2D>("p_sad");

            _qEmoteSound = _content.Load<SoundEffect>("jump (1)");
            _pEmoteSound = _content.Load<SoundEffect>("jump (4)");
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

            if (_qPlaySound.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _qEmoteSound.Play();
            }
            if (_q.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _qEmote = true;
            }
            else
            {
                _qEmote = false;
            }
            if (_pPlaySound.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _pEmoteSound.Play();
            }
            if (_p.Occurred(input, ControllingPlayer, out playerIndex))
            {
                _pEmote = true;
            }
            else
            {
                _pEmote = false;
            }
            if (_transitionTimer < 0)
            {
                LoadingScreen.Load(ScreenManager, false, new ShakeHandsScreen());
            }
            _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_podium, new Vector2(960, 960), new Rectangle(ScreenManager.QPoints > ScreenManager.PPoints ? 0 : 64, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(ScreenManager.QPoints > ScreenManager.PPoints ? _qCele : _qSad, new Vector2(627, ScreenManager.QPoints > ScreenManager.PPoints ? 482 : 722), new Rectangle(_qEmote ? 32 : 0, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(ScreenManager.PPoints > ScreenManager.QPoints ? _pCele : _pSad, new Vector2(850, ScreenManager.QPoints < ScreenManager.PPoints ? 482 : 722), new Rectangle(_pEmote ? 32 : 0, 0, 32, 32), Color.White, 0, new Vector2(32, 32), 15.0f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
