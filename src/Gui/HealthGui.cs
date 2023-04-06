using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Meridian2.GameElements;

namespace Meridian2.Gui
{
    public class HealthGui : DrawableGameElement {
        private RopeGame _game;
        private GameData _data;

        private Texture2D heartTexture;

        public HealthGui(RopeGame game, GameData data) {
            _game = game;
            _data = data;
        }

        public void LoadContent() {
            heartTexture = _game.Content.Load<Texture2D>("heart");
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
        {
            for (int i = 0; i < _data.health; i++) {
                Rectangle pos = new Rectangle(70+i*40, 10, 30, 30);
                batch.Draw(heartTexture, pos, Color.Red);
            }
        }
    }
}