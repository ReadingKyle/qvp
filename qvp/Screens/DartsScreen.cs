using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using qvp.Players;
using qvp.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace qvp.Screens
{
    public class DartsScreen : GameScreen
    {
        private ContentManager _content;

        private InputAction _pause;
        private InputAction _q;
        private InputAction _p;

        private List<DartPlayer> _dartsQ;
        private List<DartPlayer> _dartsP;

        private Texture2D _background;
        private Texture2D _text;

        SoundEffectInstance _qThrowSound;
        SoundEffectInstance _pThrowSound;

        private List<WheelSegment> _qWheel;
        private List<WheelSegment> _pWheel;

        private double _transitionTimer = 3;

        float _wheelRotation;

        private int _qScore;
        private int _pScore;

        private bool _qWins;
        private bool _pWins;

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

            _background = _content.Load<Texture2D>("bg_mainmenu");
            _text = _content.Load<Texture2D>("darts_text");

            _dartsQ = new List<DartPlayer> { new DartPlayer(_content.Load<Texture2D>("q_throwingdart"), new Vector2(240, 480)) };
            _dartsP = new List<DartPlayer> { new DartPlayer(_content.Load<Texture2D>("p_throwingdart"), new Vector2(720, 480)) };

            _qWheel = new List<WheelSegment>
            {
                new WheelSegment(_content.Load<Texture2D>("wheel_seg1"), new Vector2(240, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg2"), new Vector2(240, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg3"), new Vector2(240, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg4"), new Vector2(240, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg5"), new Vector2(240, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg6"), new Vector2(240, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg7"), new Vector2(240, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg8"), new Vector2(240, 480)),
            };
            _pWheel = new List<WheelSegment>
            {
                new WheelSegment(_content.Load<Texture2D>("wheel_seg1"), new Vector2(720, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg2"), new Vector2(720, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg3"), new Vector2(720, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg4"), new Vector2(720, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg5"), new Vector2(720, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg6"), new Vector2(720, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg7"), new Vector2(720, 480)),
                new WheelSegment(_content.Load<Texture2D>("wheel_seg8"), new Vector2(720, 480)),
            };

            SoundEffect qThrowSound = _content.Load<SoundEffect>("jump");
            SoundEffect pThrowSound = _content.Load<SoundEffect>("jump (4)");
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

        private void HandleWheelHit(List<WheelSegment> wheel, out bool newHit)
        {
            if (_wheelRotation >= 0 && _wheelRotation < Math.PI / 4 && !wheel[6].IsHit)
            {
                wheel[6].IsHit = true;
                newHit = true;
            }
            else if (_wheelRotation >= Math.PI / 4 && _wheelRotation < Math.PI / 2 && !wheel[5].IsHit)
            {
                wheel[5].IsHit = true;
                newHit = true;
            }
            else if (_wheelRotation >= Math.PI / 2 && _wheelRotation < 3 * Math.PI / 4 && !wheel[4].IsHit)
            {
                wheel[4].IsHit = true;
                newHit = true;
            }
            else if (_wheelRotation >= 3 * Math.PI / 4 && _wheelRotation < Math.PI && !wheel[3].IsHit)
            {
                wheel[3].IsHit = true;
                newHit = true;
            }
            else if (_wheelRotation >= Math.PI && _wheelRotation < 5 * Math.PI / 4 && !wheel[2].IsHit)
            {
                wheel[2].IsHit = true;
                newHit = true;
            }
            else if (_wheelRotation >= 5 * Math.PI / 4 && _wheelRotation < 3 * Math.PI / 2 && !wheel[1].IsHit)
            {
                wheel[1].IsHit = true;
                newHit = true;
            }
            else if (_wheelRotation >= 3 * Math.PI / 2 && _wheelRotation < 7 * Math.PI / 4 && !wheel[0].IsHit)
            {
                wheel[0].IsHit = true;
                newHit = true;
            }
            else if (_wheelRotation >= 7 * Math.PI / 4 && _wheelRotation < 2 * Math.PI && !wheel[7].IsHit)
            {
                wheel[7].IsHit = true;
                newHit = true;
            }
            else
            {
                newHit = false;
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            if (_qScore == 8 || _pScore == 8)
            {
                _transitionTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_transitionTimer < 0)
            {
                if (_qWins) ScreenManager.QPoints++;
                else ScreenManager.PPoints++;
                LoadingScreen.Load(ScreenManager, false, new TransitionScreen());
            }
            if (_pause.Occurred(input, ControllingPlayer, out playerIndex))
            {
                var pauseScreen = new MessageBoxScreen("PAUSE\n\npress (ESC) to resume\npress (SPACE) to exit");
                ScreenManager.AddScreen(pauseScreen);
                pauseScreen.Accepted += ConfirmExitMessageBoxAccepted;
            }
            if (_q.Occurred(input, ControllingPlayer, out playerIndex) && !_pWins)
            {
                _qThrowSound.Play();
                _dartsQ[_dartsQ.Count - 1].IsThrowing = true;
                _dartsQ[_dartsQ.Count - 1].Update(1);
                bool newHit = false;
                HandleWheelHit(_qWheel, out newHit);
                if (newHit) _qScore++;
            }
            if (_dartsQ[_dartsQ.Count - 1].IsStuck)
            {
                _dartsQ.Add(new DartPlayer(_content.Load<Texture2D>("q_throwingdart"), new Vector2(240, 480)));
            }
            if (_p.Occurred(input, ControllingPlayer, out playerIndex) && !_qWins)
            {
                _pThrowSound.Play();
                _dartsP[_dartsP.Count - 1].IsThrowing = true;
                _dartsP[_dartsP.Count - 1].Update(1);
                bool newHit = false;
                HandleWheelHit(_pWheel, out newHit);
                if (newHit) _pScore++;
            }
            if (_dartsP[_dartsP.Count - 1].IsStuck)
            {
                _dartsP.Add(new DartPlayer(_content.Load<Texture2D>("p_throwingdart"), new Vector2(720, 480)));
            }
            foreach (DartPlayer dart in _dartsQ)
            {
                if (dart.IsStuck) dart.Rotation += (float)(Math.PI) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            foreach (DartPlayer dart in _dartsP)
            {
                if (dart.IsStuck) dart.Rotation += (float)(Math.PI) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_qScore == 8) _qWins = true;
            if (_pScore == 8) _pWins = true;

            _wheelRotation += (float)(Math.PI) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_wheelRotation >= 2*Math.PI) _wheelRotation = 0f;
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.Draw(_text, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);

            foreach (WheelSegment seg in _qWheel) seg.Draw(spriteBatch, gameTime, _wheelRotation);
            foreach (DartPlayer dart in _dartsQ) dart.Draw(spriteBatch, gameTime, _wheelRotation);

            foreach (WheelSegment seg in _pWheel) seg.Draw(spriteBatch, gameTime, _wheelRotation);
            foreach (DartPlayer dart in _dartsP) dart.Draw(spriteBatch, gameTime, _wheelRotation);
            spriteBatch.End();
        }
    }
}
