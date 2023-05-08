using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui; 

public class StatsGui : DrawableGameElement {
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _enemyTexture;
    private SpriteFont _font;

    public StatsGui(RopeGame game, GameData data) {
        _game = game;
        _data = data;
    }

    public void LoadContent() {
        _enemyTexture = _game.Content.Load<Texture2D>("Sprites/Enemies/enemy");
        _font = _game.Content.Load<SpriteFont>("Damn");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var viewportWidth = _game.GraphicsDevice.Viewport.Width;

        var text = _data.Kills.ToString();
        var stringSize = _font.MeasureString(_data.Kills.ToString());
        var margin = 10;
        var enemyWidth = (int)(((float)_enemyTexture.Width / (float)_enemyTexture.Height) * stringSize.Y);
        var enemyTextureRect = new Rectangle(viewportWidth - margin - enemyWidth, -8, enemyWidth, (int)stringSize.Y);
        var position = new Vector2(viewportWidth - stringSize.X - 2*margin - enemyTextureRect.Width, margin);
        
        batch.DrawString(_font, text, position, new(170, 54, 54), 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        batch.Draw(_enemyTexture, enemyTextureRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
    }
}