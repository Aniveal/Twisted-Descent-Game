using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwistedDescent.Screens;

public class HighScoreScreen : Screen
{
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    private Texture2D _playerModel;

    private Texture2D _bg;

    private Texture2D LB;
    private Texture2D RB;
    private Texture2D X;
    private Texture2D RT;
    private Texture2D A;
    private Texture2D Left_Stick;

    private Texture2D Q;
    private Texture2D E;
    private Texture2D R;
    private Texture2D P;
    private Texture2D Space;
    private Texture2D WASD;
    private Texture2D Arrow_Keys;

    public Boolean gameLoaded;
    double timer;
    private SpriteFont font;
    private Color font_color;

    private int w;
    private int h;

    public HighScoreScreen(RopeGame game, ContentManager content) : base(game)
    {
        font = content.Load<SpriteFont>("Fonts/control_screen_text");
        font_color = new Color(154, 134, 129);

        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");

        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        gameLoaded = false;
        timer = 0;

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        // Draw Background:
        spriteBatch.Draw(_bg, new Rectangle(0, 0, w, h), Color.White);

        // Write Text:
        int font_height = (int)font.MeasureString("SCORE").Y;
        int font_width = (int)font.MeasureString("SCORE").X;
        int start_name_width = w / 2;
        int start_name_height = h / 16;

        int index = 0;
        spriteBatch.DrawString(font, "Rank", new Vector2(start_name_width - 3 * font_width, start_name_height + index * font_height), font_color);
        spriteBatch.DrawString(font, "Name", new Vector2(start_name_width - 2 * font_width, start_name_height + index * font_height), font_color);
        spriteBatch.DrawString(font, "Score", new Vector2(start_name_width, start_name_height + index * font_height), font_color);
        spriteBatch.DrawString(font, "Kills", new Vector2(start_name_width + 2 * font_width, start_name_height + index * font_height), font_color);
        spriteBatch.DrawString(font, "Level", new Vector2(start_name_width + 3 * font_width, start_name_height + index * font_height), font_color);
        foreach (var item in _game.leaderBoard)
        {
            index += 1;
            spriteBatch.DrawString(font, index.ToString() + ". ", new Vector2(start_name_width - 3 * font_width, start_name_height + index * font_height), font_color);
            spriteBatch.DrawString(font, item.Item1, new Vector2(start_name_width - 2 * font_width, start_name_height + index * font_height), font_color);
            spriteBatch.DrawString(font, item.Item2.ToString(), new Vector2(start_name_width, start_name_height + index * font_height), font_color);
            spriteBatch.DrawString(font, item.Item3.ToString(), new Vector2(start_name_width + 2 * font_width, start_name_height + index * font_height), font_color);
            spriteBatch.DrawString(font, item.Item4.ToString(), new Vector2(start_name_width + 3 * font_width, start_name_height + index * font_height), font_color);
        }

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        //
    }

}