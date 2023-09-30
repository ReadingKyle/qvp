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

        TimeSpan _displayTime;

        public override void Activate()
        {
            base.Activate();

            if (_content == null) _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _background = _content.Load<Texture2D>("handshake_bg");

            Random random = new Random();
            _playerQ = random.Next(0, 2) == 0 ? _content.Load<Texture2D>("q_pose_boxer") : _content.Load<Texture2D>("q_pose_karate");
            _playerP = random.Next(0, 2) == 0 ? _content.Load<Texture2D>("p_pose_boxer") : _content.Load<Texture2D>("p_pose_karate");

            _circleEmpty = _content.Load<Texture2D>("circle_empty");
            _circleBlue = _content.Load<Texture2D>("circle_blue");
            _circlePink = _content.Load<Texture2D>("circle_pink");

            _displayTime = TimeSpan.FromSeconds(3);

            ScreenManager.Game.ResetElapsedTime();
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);

            _displayTime -= gameTime.ElapsedGameTime;
            if (_displayTime <= TimeSpan.Zero)
            {
                if (ScreenManager.GameIndex >= ScreenManager.Games.Count-1 || ScreenManager.QPoints == 2 || ScreenManager.PPoints == 2)
                {
                    ScreenManager.GameIndex = -1;
                    LoadingScreen.Load(ScreenManager, false, new PodiumScreen());
                }
                else
                {
                    ScreenManager.GameIndex++;
                    LoadingScreen.Load(ScreenManager, false, ScreenManager.Games[ScreenManager.GameIndex]);
                }
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            ScreenManager.SpriteBatch.Draw(_background, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.Draw(ScreenManager.QPoints > 0 ? _circleBlue : _circleEmpty, new Vector2(340, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            if (ScreenManager.QPoints >= 2)
            {
                ScreenManager.SpriteBatch.Draw(_circleBlue, new Vector2(540, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            }
            else if (ScreenManager.PPoints >= 2)
            {
                ScreenManager.SpriteBatch.Draw(_circlePink, new Vector2(540, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(_circleEmpty, new Vector2(540, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            }
            ScreenManager.SpriteBatch.Draw(ScreenManager.PPoints > 0 ? _circlePink : _circleEmpty, new Vector2(740, 460), new Rectangle(0, 0, 8, 8), Color.White, 0, new Vector2(8, 8), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.Draw(_playerQ, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.Draw(_playerP, new Vector2(960, 960), new Rectangle(0, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            ScreenManager.SpriteBatch.End();
        }


    }
}
