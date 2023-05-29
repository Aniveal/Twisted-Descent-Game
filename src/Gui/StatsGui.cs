using System;
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
    private Texture2D _level_background;
    private SpriteFont _font;
    private SpriteFont _msg_font;

    private int max_skulls;
    private int skull_size = 42 + 5;
    private int margin = 10;

    private Color text_color = new Color(154, 139, 141);
    private Color text_background_color = new Color(34, 35, 35);
    private Color level_msg_color = new Color(207, 180, 177);
    private int text_outline_size = 1; // pixels

    private double level_msg_duration = 5000;
    private double level_animation_duration = 700;
    private int level_msg_box_margin = 6;
    private int level_box_vertical_offset = 50;


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
        _level_background = _game.Content.Load<Texture2D>("Sprites/UI/level_msg_background");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        var viewportWidth = _game.GraphicsDevice.Viewport.Width;

        // Write the Level
        // When starting a new level, a box with message "Level 1" appears. 
        
        var TimeDelta = gameTime.TotalGameTime.TotalMilliseconds - (_game.GameData.levelStartTime + 1000);
        if (TimeDelta < level_msg_duration && TimeDelta > 0)
        {
            var level_text = "Level " + _game._gameScreen._map.mapLevel;
            if (_game.GameData.tutorial)
                level_text = "Tutorial";

            var level_stringSize = _font.MeasureString(level_text);
            
            var box_width = (int)(level_stringSize.X + 2 * level_msg_box_margin);   // box width when animation is done
            Color animated_level_msg_color = level_msg_color;                       // font color when animation is done   

            // Animate Level Message:
            if (TimeDelta < level_animation_duration)   // fade in animation: box stretches outwards (x-axis), color lerps from background grey to font color
            {
                var animation_factor = (float) Math.Min(1f, TimeDelta / level_animation_duration);  // 0f to 1f, how far we're in animation (linear)
                box_width = (int)(animation_factor * (float) box_width);    // new width of the message box
                animated_level_msg_color = Color.Lerp(text_background_color, level_msg_color, animation_factor); // new color
            }

            else if (TimeDelta > level_msg_duration - level_animation_duration) // fade out animation: box collapses inwards (x-axis), color lerps back to background grey
            {
                var animation_time = level_msg_duration - TimeDelta;
                var animation_factor = (float) Math.Min(1f, animation_time / level_animation_duration); // 1f to 0f, how far we're in animation (linear)
                box_width = (int)(animation_factor * (float)box_width); // new width of the message box
                animated_level_msg_color = Color.Lerp(text_background_color, level_msg_color, animation_factor); // new color
            }

            // Level Message Positions:
            var level_msg_box_position = new Rectangle(
                    (int)(viewportWidth / 2f - box_width / 2),
                    level_box_vertical_offset + level_msg_box_margin,
                    box_width,
                    (int)level_stringSize.Y + 2 * level_msg_box_margin
                );

            var level_msg_string_pos = new Vector2(
                    (int)(viewportWidth / 2f - level_stringSize.X / 2),
                    level_box_vertical_offset + level_msg_box_margin + 14
                );

            // Drawing level message:
            batch.Draw(_level_background, level_msg_box_position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
            batch.DrawString(_font, level_text, level_msg_string_pos, animated_level_msg_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }

        // Killstreak
        var killstreak = _data.updateKillStreak(gameTime.TotalGameTime.TotalMilliseconds);
        
        if (killstreak > 1)
        {
            var msg = KillstreakMessage(killstreak);
            var msgSize = _font.MeasureString(msg);
            // old Color: new(170, 54, 54)
            DrawStringWithOutline(batch, _msg_font, msg, new Vector2(viewportWidth / 2f - msgSize.X / 2, margin + msgSize.Y), 2, new(170, 54, 54), text_background_color);
            
        }

        // Writing the number of Skills as a number
        var text = _data.Kills.ToString();
        var stringSize = _font.MeasureString(text);
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
                batch.Draw(skull_sprite, skull_pos, null, skull_color, 0f, Vector2.Zero, SpriteEffects.None, 1f - i * 0.0001f);
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
                msg = "Diplo-Kill!";
                break;
            case 3:
                msg = "Triplo-Kill!";
                break;
            case 4:
                msg = "Tetraplo-Kill!";
                break;
            case 5:
                msg = "Pentaplo-Kill!";
                break;
            case 6:
                msg = "Hexaplo-Kill!";
                break;
            case 7:
                msg = "Heptaplo-Kill!";
                break;
            case 8:
                msg = "Octaplo-Kill!";
                break;
            case 9:
                msg = "Enneaplo-Kill!";
                break;
            case 10:
                msg = "Decaplo-Kill!";
                break;
            case > 10:
                msg = killstreak + "-Kills!";
                break;
        }
        return msg;
    }
}