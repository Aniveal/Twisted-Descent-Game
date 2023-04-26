using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui; 

public class HealthGui : DrawableGameElement {
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _heartTexture;

    public HealthGui(RopeGame game, GameData data) {
        _game = game;
        _data = data;
    }

    public void LoadContent() {
        _heartTexture = _game.Content.Load<Texture2D>("Sprites/UI/heart");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        if (_data.GameOver) {
            var ropeRed = new Color(170, 54, 54);
            batch.DrawString(_game.Font, "GAME  OVER", new Vector2(300, 300), ropeRed, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 1f);
            return;
        }

        for (var i = 0; i < _data.Health; i++) {
            var pos = new Rectangle(70 + i * 40, 10, 30, 30);
            batch.Draw(_heartTexture, pos, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
    }
}