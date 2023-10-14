using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
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
    public class TransitionScreen : GameScreen
    {
        private ContentManager _content;
        private Texture2D _background;
        private Texture2D _circleEmpty;
        private Texture2D _circleBlue;
        private Texture2D _circlePink;
        private Texture2D _playerQ;
        private Texture2D _playerP;

        private Texture2D _bgQ;
        private Texture2D _bgP;

        TimeSpan _displayTime;

        private Vector2 _leftVelocity;
        private Vector2 _rightVelocity;
        private Vector2 _upVelocity;

        private Vector2 _playerQPosition;
        private Vector2 _playerPPosition;
        private Vector2 _pointsPosition;
        private Vector2 _bgQPosition;
        private Vector2 _bgPPosition;

        public override void Activate()
        {
            base.Activate();

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("handshake_bg");
            _bgQ = _content.Load<Texture2D>("transition_q_bg");
            _bgP = _content.Load<Texture2D>("transition_p_bg");

            Random random = new Random();
            _playerQ = random.Next(0, 2) == 0 ? _content.Load<Texture2D>("q_pose_boxer") : _content.Load<Texture2D>("q_pose_karate");
            _playerP = random.Next(0, 2) == 0 ? _content.Load<Texture2D>("p_pose_boxer") : _content.Load<Texture2D>("p_pose_karate");

            _circleEmpty = _content.Load<Texture2D>("circle_empty");
            _circleBlue = _content.Load<Texture2D>("circle_blue");
            _circlePink = _content.Load<Texture2D>("circle_pink");

            _displayTime = TimeSpan.FromSeconds(3);

            ScreenManager.Game.ResetElapsedTime();

            /*_playerQPosition = new Vector2(480, 960);
            _playerPPosition = new Vector2(1440, 960);
            _pointsPosition = new Vector2(0, -1440);
            _bgQPosition = new Vector2(480, 960);
            _bgPPosition = new Vector2(1440, 960);

            _leftVelocity = new Vector2(8.15f, 0);
            _rightVelocity = new Vector2(-8.15f, 0);
            _upVelocity = new Vector2(0, 15);*/

            _playerQPosition = new Vector2(960, 960);
            _playerPPosition = new Vector2(960, 960);
            _pointsPosition = new Vector2(0, 0);
            _bgQPosition = new Vector2(960, 960);
            _bgPPosition = new Vector2(960, 960);

            _leftVelocity = new Vector2(0, 0);
            _rightVelocity = new Vector2(0, 0);
            _upVelocity = new Vector2(0, 0);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);

            _displayTime -= gameTime.ElapsedGameTime;
            if (_displayTime <= TimeSpan.Zero)
            {
                if (ScreenManager.GameIndex >= ScreenManager.Games.Count-1 || ScreenManager.QPoints == 3 || ScreenManager.PPoints == 3)
                {
                    ScreenManager.GameIndex = -1;
                    LoadingScreen.Load(ScreenManager, true, new PodiumScreen());
                }
                else
                {
                    ScreenManager.GameIndex++;
                    LoadingScreen.Load(ScreenManager, true, ScreenManager.Games[ScreenManager.GameIndex]);
                }
                //ExitScreen();
            }
            if (_displayTime < TimeSpan.FromSeconds(2) && _displayTime > TimeSpan.FromSeconds(1))
            {
/*                _leftVelocity = new Vector2(0, 0);
                _rightVelocity = new Vector2(0, 0);
                _upVelocity = new Vector2(0, 0);*/
            }
            if (_displayTime < TimeSpan.FromSeconds(1))
            {
/*                _leftVelocity = new Vector2(-8.15f, 0);
                _rightVelocity = new Vector2(8.15f, 0);
                _upVelocity = new Vector2(0, -15);*/
            }

            _playerQPosition += _leftVelocity;
            _bgQPosition += _leftVelocity;
            _playerPPosition += _rightVelocity;
            _bgPPosition += _rightVelocity;
            _pointsPosition += _upVelocity;

        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            ScreenManager.SpriteBatch.Draw(_bgQ, _bgQPosition, new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.Draw(_bgP, _bgPPosition, new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();

            Matrix transform;
            transform = Matrix.CreateTranslation(0, _pointsPosition.Y * 0.333f, 0);
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, transformMatrix: transform);
            ScreenManager.SpriteBatch.Draw(ScreenManager.QPoints > 0 ? _circleBlue : _circleEmpty, new Vector2(140, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.Draw(ScreenManager.QPoints > 1 ? _circleBlue : _circleEmpty, new Vector2(340, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            if (ScreenManager.QPoints == 3)
            {
                ScreenManager.SpriteBatch.Draw(_circleBlue, new Vector2(540, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            }
            else if (ScreenManager.PPoints == 3)
            {
                ScreenManager.SpriteBatch.Draw(_circlePink, new Vector2(540, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(_circleEmpty, new Vector2(540, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            }
            ScreenManager.SpriteBatch.Draw(ScreenManager.PPoints > 1 ? _circlePink : _circleEmpty, new Vector2(740, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.Draw(ScreenManager.PPoints > 0 ? _circlePink : _circleEmpty, new Vector2(940, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            ScreenManager.SpriteBatch.Draw(_playerQ, _playerQPosition + _leftVelocity, new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.Draw(_playerP, _playerPPosition + _rightVelocity, new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
        }


    }
}
