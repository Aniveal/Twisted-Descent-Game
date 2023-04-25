using Meridian2.Columns;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui; 

public class GuiManager {
    private readonly RopeGame _game;
    private readonly HealthGui _healthGui;
    private readonly SpearsGui _spearsGui;

    public GuiManager(RopeGame game, SpearsController spearsController) {
        _game = game;
        _healthGui = new HealthGui(_game, _game.GameData);
        _spearsGui = new SpearsGui(_game, _game.GameData, spearsController);
    }

    public void Initialize() { }

    public void LoadContent() {
        _healthGui.LoadContent();
        _spearsGui.LoadContent();
    }

    /**
     * Implemented for the sake of implementing it, will probablybe removed later
     * GUI should not perform updates but obtain needed data from other elements
     */
    public void Update(GameTime gameTime) {
        _healthGui.Update(gameTime);
        _spearsGui.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        _healthGui.Draw(gameTime, batch, camera);
        _spearsGui.Draw(gameTime, batch, camera);
    }
}