using System;
using System.Diagnostics;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui;

public class StatsGui : DrawableGameElement
{
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _enemyTexture;
    private Texture2D _skull;
    private Texture2D _skull_1;
    private Texture2D _skull_2;
    private SpriteFont _font;

    private int max_skulls;
    private int skull_size;

    public StatsGui(RopeGame game, GameData data)
    {
        _game = game;
        _data = data;

        skull_size = 42 + 5;

        // maximum number of skulls shown depends on the width of the screen
        var viewportWidth = _game.GraphicsDevice.Viewport.Width;
        max_skulls = (int)((viewportWidth / 2) / (5 * skull_size / 8));

    }

    public void LoadContent()
    {
        _enemyTexture = _game.Content.Load<Texture2D>("Sprites/Enemies/enemy");
        _skull = _game.Content.Load<Texture2D>("Sprites/UI/skull_icon");
        _skull_1 = _game.Content.Load<Texture2D>("Sprites/UI/skull_icon_1");
        _skull_2 = _game.Content.Load<Texture2D>("Sprites/UI/skull_icon_2");
        _font = _game.Content.Load<SpriteFont>("Fonts/damn_ui");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        var viewportWidth = _game.GraphicsDevice.Viewport.Width;
        var margin = 10;

        // Write the level
        var text = "Level " + _game._gameScreen._map.mapLevel;
        var stringSize = _font.MeasureString(text);
        batch.DrawString(_font, text, new Vector2(viewportWidth / 2f - stringSize.X / 2, margin), new(170, 54, 54), 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        
        // Writing the number of Skills as a number
        text = _data.Kills.ToString();
        stringSize = _font.MeasureString(text);
        var text_width = (int) Math.Max(stringSize.X, _font.MeasureString("42").X); // leaving space for up to 99 kills

        if (text.Length < 2)    // right align text (after measuring its size!)
            text = " " + text;

        var text_position = new Vector2(margin, margin);
        var text_color = new Color(154, 139, 141);
        var text_background_color = new Color(34, 35, 35);
        var text_outline_size = 1; // pixels

        batch.DrawString(_font, text, text_position, text_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Drawing a black outline around the text:
        batch.DrawString(_font, text, new Vector2(text_position.X - text_outline_size, text_position.Y - text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X - text_outline_size, text_position.Y + text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X + text_outline_size, text_position.Y - text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(_font, text, new Vector2(text_position.X + text_outline_size, text_position.Y + text_outline_size), text_background_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);


        // Drawing Skull Icons
        var n_skulls = Math.Min(max_skulls, _data.Kills);

        if (n_skulls == 0)  // if player has no kills yet, draw grayed-out skull
        {
            var skull_pos = new Rectangle(2 * margin + text_width, margin, skull_size, skull_size);
            var skull_color = new Color(100, 100, 100);
            batch.Draw(_skull, skull_pos, null, skull_color, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
        else
        {
            for (var i = 0; i < n_skulls; i++)
            {
                var skull_pos = new Rectangle(2 * margin + text_width + (i * 5 * skull_size / 8), margin, skull_size, skull_size);
                var skull_color = Color.White;
                var skull_sprite = _skull;
                if (i % 3 == 1)
                {
                    skull_sprite = _skull_1;
                }
                else if (i % 3 == 2)
                {
                    skull_sprite = _skull_2;
                }
                batch.Draw(skull_sprite, skull_pos, null, skull_color, 0f, Vector2.Zero, SpriteEffects.None, 1f - i * 0.001f);
            }
        }
    }
}