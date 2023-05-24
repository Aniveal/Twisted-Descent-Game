using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwistedDescent.GameElements;

namespace TwistedDescent.Gui;

public class HealthGui : DrawableGameElement
{
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _heartTexture;
    private SpriteFont _font;
    private SpriteFont _DeathMessageFont;

    private Color text_color = new Color(170, 54, 54);
    private Color text_outline_color = new Color(34, 35, 35);
    private Color lost_heart_color = new Color(83, 85, 85);

    private int text_outline_size = 1;      // pixels
    private int heart_size = 42 + 5;        // pixels, 42 is font size, + 5 to make it look nicer

    public HealthGui(RopeGame game, GameData data)
    {
        _game = game;
        _data = data;
    }

    public void LoadContent()
    {
        _heartTexture = _game.Content.Load<Texture2D>("Sprites/UI/hearth_icon");
        _font = _game.Content.Load<SpriteFont>("Fonts/damn_ui");
        _DeathMessageFont = _game.Content.Load<SpriteFont>("Fonts/death_message_font");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        if (_data.GameOver && _game.GameData.TimeLeft < -0.5f)
        {
            var msg = "You Died";
            var msgSize = _font.MeasureString(msg);
            var ropeRed = new Color(170, 54, 54);
            var deathMessagePos = new Vector2((_game.GraphicsDevice.Viewport.Width - msgSize.X) / 2, _game.GraphicsDevice.Viewport.Height / 4);
            DrawStringWithOutline(batch, _font, msg, deathMessagePos, 3, ropeRed, text_outline_color);
            return;
        }

        var margin = 10;
        var text = _data.Health.ToString();
        var stringSize = _font.MeasureString(text);
        var text_width = (int)Math.Max(stringSize.X, _font.MeasureString("42").X); // leaving space for up to 99 kills

        if (text.Length < 2) // right align text (after measuring its size!)
            text = " " + text;

        var text_position = new Vector2(margin, (int)(5 * margin / 4 + stringSize.Y));

        batch.DrawString(_font, text, text_position, text_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Drawing a black outline around the text:
        batch.DrawString(_font, text, new Vector2(text_position.X - text_outline_size, text_position.Y - text_outline_size), text_outline_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X - text_outline_size, text_position.Y + text_outline_size), text_outline_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X + text_outline_size, text_position.Y - text_outline_size), text_outline_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X + text_outline_size, text_position.Y + text_outline_size), text_outline_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);

        for (var i = 0; i < _data.MaxHealth; i++)
        {
            var texture_color = (i < _data.Health) ? Color.White : lost_heart_color;
            var pos = new Rectangle(2 * margin + text_width + i * heart_size, (int)text_position.Y, heart_size, heart_size);

            batch.Draw(_heartTexture, pos, null, texture_color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
    }

    private void DrawStringWithOutline(SpriteBatch batch, SpriteFont font, String text, Vector2 position, int outlineSize, Color fontColor, Color outlineColor)
    {
        batch.DrawString(font, text, position, fontColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Drawing a black outline around the text:
        batch.DrawString(font, text, new Vector2(position.X - outlineSize, position.Y - outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(font, text, new Vector2(position.X - outlineSize, position.Y + outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(font, text, new Vector2(position.X + outlineSize, position.Y - outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(font, text, new Vector2(position.X + outlineSize, position.Y + outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
    }
}