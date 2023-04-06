using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Meridian2.Gui {
    public class GuiManager {

        private RopeGame _game;
        private HealthGui _healthGui;
        public GuiManager(RopeGame game) {
            _game = game;
            _healthGui = new HealthGui(_game, _game.gameData);
        }

        public void Initialize() {

        }

        public void LoadContent() {
            _healthGui.LoadContent();
        }

        /**
        *   Implemented for the sake of implementing it, will probablybe removed later
        *   GUI should not perform updates but obtain needed data from other elements
        */
        public void Update(GameTime gameTime) {
            _healthGui.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
            _healthGui.Draw(gameTime, batch, camera);
        }
    }
}