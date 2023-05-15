using System;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui;

public class HealthGui : DrawableGameElement
{
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _heartTexture;
    private SpriteFont _font;

    private int heart_size;

    public HealthGui(RopeGame game, GameData data)
    {
        _game = game;
        _data = data;

        heart_size = 42 + 5;
    }

    public void LoadContent()
    {
        _heartTexture = _game.Content.Load<Texture2D>("Sprites/UI/hearth_icon");
        _font = _game.Content.Load<SpriteFont>("damn_ui");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        if (_data.GameOver)
        {
            var ropeRed = new Color(170, 54, 54);
            batch.DrawString(_game.Font, "GAME  OVER", new Vector2(300, 300), ropeRed, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 1f);
            return;
        }

        var margin = 10;
        var text = _data.Health.ToString();
        var stringSize = _font.MeasureString(text);
        var text_width = (int)Math.Max(stringSize.X, _font.MeasureString("42").X); // leaving space for up to 99 kills

        if (text.Length < 2) // right align text (after measuring its size!)
            text = " " + text;

        var text_position = new Vector2(margin, (int)(5 * margin / 4 + stringSize.Y));
        var text_color = new Color(170, 54, 54);
        var text_background_color = new Color(34, 35, 35);
        var text_outline_size = 1; // pixels

        batch.DrawString(_font, text, text_position, text_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Drawing a black outline around the text:
        batch.DrawString(_font, text, new Vector2(text_position.X - text_outline_size, text_position.Y - text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X - text_outline_size, text_position.Y + text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X + text_outline_size, text_position.Y - text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X + text_outline_size, text_position.Y + text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);

        for (var i = 0; i < _data.MaxHealth; i++)
        {
            Color color = i < _data.Health ? Color.White : Color.Black;
            var pos = new Rectangle(2 * margin + text_width + i * heart_size, (int)text_position.Y, heart_size, heart_size);
            batch.Draw(_heartTexture, pos, null, color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
    }
}