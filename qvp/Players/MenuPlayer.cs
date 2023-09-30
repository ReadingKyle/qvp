using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace qvp.Players
{
    public class MenuPlayer
    {
        private Vector2 _position;
        private Texture2D _texture;
        public bool IsPressing;

        public MenuPlayer(Vector2 Position, Texture2D texture)
        {
            _position = Position;
            _texture = texture;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            int selectedFrame = IsPressing ? 64 : 0;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(_texture, new Vector2(960, 960), new Rectangle(0 + selectedFrame, 0, 64, 64), Color.White, 0, new Vector2(64, 64), 15.0f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

    }
}
