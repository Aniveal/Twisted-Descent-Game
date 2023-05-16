﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwistedDescent.GameElements;
using static System.Net.Mime.MediaTypeNames;

namespace TwistedDescent.Gui;

public class StatsGui : DrawableGameElement
{
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _enemyTexture;
    private Texture2D _skull;
    private Texture2D _skull_1;
    private Texture2D _skull_2;
    private SpriteFont _font;
    private SpriteFont _msg_font;

    private int max_skulls;
    private int skull_size = 42 + 5;
    private int margin = 10;

    private Color text_color = new Color(154, 139, 141);
    private Color text_background_color = new Color(34, 35, 35);
    private int text_outline_size = 1; // pixels

    public StatsGui(RopeGame game, GameData data)
    {
        _game = game;
        _data = data;

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
        _msg_font = _game.Content.Load<SpriteFont>("Fonts/damn");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        var viewportWidth = _game.GraphicsDevice.Viewport.Width;
        
        // Write the level
        var text = "Level " + _game._gameScreen._map.mapLevel;
        var stringSize = _font.MeasureString(text);
        batch.DrawString(_font, text, new Vector2(viewportWidth / 2f - stringSize.X / 2, margin), new(170, 54, 54), 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        // Killstreak
        var killstreak = _data.updateKillStreak(gameTime.TotalGameTime.TotalMilliseconds);
        
        if (killstreak > 0)
        {
            var msg = KillstreakMessage(killstreak);
            var msgSize = _font.MeasureString(msg);
            // old Color: new(170, 54, 54)
            DrawStringWithOutline(batch, _msg_font, msg, new Vector2(viewportWidth / 2f - msgSize.X / 2, margin + msgSize.Y), 2, new(170, 54, 54), text_background_color);
            
        }

        // Writing the number of Skills as a number
        text = _data.Kills.ToString();
        stringSize = _font.MeasureString(text);
        var text_width = (int) Math.Max(stringSize.X, _font.MeasureString("42").X); // leaving space for up to 99 kills

        if (text.Length < 2)    // right align text (after measuring its size!)
            text = " " + text;

        var text_position = new Vector2(margin, margin);

        DrawStringWithOutline(batch, _font, text, text_position, text_outline_size, text_color, text_background_color);

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

    private void DrawStringWithOutline(SpriteBatch batch, SpriteFont font, String text, Vector2 position, int outlineSize, Color fontColor, Color outlineColor)
    {
        batch.DrawString(font, text, position, fontColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // Drawing a black outline around the text:
        batch.DrawString(font, text, new Vector2(position.X - outlineSize, position.Y - outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(font, text, new Vector2(position.X - outlineSize, position.Y + outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(font, text, new Vector2(position.X + outlineSize, position.Y - outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
        batch.DrawString(font, text, new Vector2(position.X + outlineSize, position.Y + outlineSize), outlineColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
    }

    private String KillstreakMessage(int killstreak)
    {
        var msg = "";
        switch (killstreak)
        {
            case 1:
                msg = "Nice Kill!";
                break;
            case 2:
                msg = "Double-Kill!";
                break;
            case 3:
                msg = "Tetra-Kill!";
                break;
            case 4:
                msg = "Quad-Kill!";
                break;
            case 5:
                msg = "Penta-Kill!";
                break;
            case 6:
                msg = "Hexa-Kill!";
                break;
            case > 6:
                msg = killstreak + "-Kills!";
                break;
        }
        return msg;
    }
}