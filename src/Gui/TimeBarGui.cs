using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui;

public class TimeBarGui : DrawableGameElement {
    private readonly GameData _data;
    private readonly RopeGame _game;

    // The bar is 80% of screen height
    private const float BarHeightRatio = 0.8f;
    private const float FillBarWidthRatio = 0.7f;
    private const int Margin = 10;

    private Texture2D _frontTexture;
    private Texture2D _backTexture;
    private Texture2D _fillTexture;

    private readonly Color _fullColor = new(68, 156, 212);
    private readonly Color _emptyColor = new(229, 73, 73);

    public TimeBarGui(RopeGame game, GameData data) {
        _game = game;
        _data = data;
    }

    public void LoadContent() {
        _frontTexture = _game.Content.Load<Texture2D>("Sprites/UI/timebar_front");
        _backTexture = _game.Content.Load<Texture2D>("Sprites/UI/timebar_back");
        _fillTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        _fillTexture.SetData(new[] { Color.White });
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        var viewportWidth = _game.GraphicsDevice.Viewport.Width;
        var viewportHeight = _game.GraphicsDevice.Viewport.Height;

        var fillRatio = _data.TimeLeft / _data.MaxTimeLeft;
        var fillColor = Color.Lerp(_emptyColor, _fullColor, (float)fillRatio);

        var barHeight = BarHeightRatio * viewportHeight;
        var barWidth = (float)_frontTexture.Width / (float)_frontTexture.Height * barHeight;
        var timeBarRect = new Rectangle(viewportWidth - (int)barWidth - Margin, viewportHeight - (int)barHeight, (int)barWidth, (int)barHeight);

        var fillHeight = BarHeightRatio * viewportHeight * fillRatio;
        var fillWidth = barWidth * FillBarWidthRatio;
        var fillBarCentering = (barWidth - fillWidth) / 2;
        var timeFillRect = new Rectangle(viewportWidth - (int)barWidth - Margin + (int)fillBarCentering, viewportHeight - (int)fillHeight, (int)fillWidth, (int)fillHeight);

        batch.Draw(_backTexture, timeBarRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
        batch.Draw(_fillTexture, timeFillRect, null, fillColor, 0f, Vector2.Zero, SpriteEffects.None, 0.995f);
        batch.Draw(_frontTexture, timeBarRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
    }
}