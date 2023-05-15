using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwistedDescent.Screens; 

public class ControlScreen : Screen {
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

    public ControlScreen(RopeGame game, ContentManager content) : base(game)
    {
        font = content.Load<SpriteFont>("Fonts/control_screen_text");
        font_color = new Color(154, 134, 129);

        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");

        LB = content.Load<Texture2D>("Sprites/Controller/LB");
        RB = content.Load<Texture2D>("Sprites/Controller/RB");
        X = content.Load<Texture2D>("Sprites/Controller/X");
        RT = content.Load<Texture2D>("Sprites/Controller/RT");
        A = content.Load<Texture2D>("Sprites/Controller/A");
        Left_Stick = content.Load<Texture2D>("Sprites/Controller/Left_Stick");

        Q = content.Load<Texture2D>("Sprites/Controller/Q");
        E = content.Load<Texture2D>("Sprites/Controller/E");
        R = content.Load<Texture2D>("Sprites/Controller/R");
        P = content.Load<Texture2D>("Sprites/Controller/P");
        Space = content.Load<Texture2D>("Sprites/Controller/Space");
        WASD = content.Load<Texture2D>("Sprites/Controller/WASD");
        Arrow_Keys = content.Load<Texture2D>("Sprites/Controller/Arrow_Keys");

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
        int font_height = (int)font.MeasureString("Test").Y;
        int horizontal_margin = w / 16;
        int vertical_margin = h / 16;

        String intro_text_1 = "In this game you are playing as Theseus (";

        spriteBatch.DrawString(font, intro_text_1, new Vector2(horizontal_margin, vertical_margin), font_color);
        spriteBatch.Draw(_playerModel, new Rectangle(horizontal_margin + (int)font.MeasureString(intro_text_1).X, vertical_margin, 5 * font_height / 9, font_height), Color.White);
        spriteBatch.DrawString(font, ")", new Vector2(horizontal_margin + (int)font.MeasureString(intro_text_1).X + 5 * font_height / 9, vertical_margin), font_color);

        spriteBatch.DrawString(font, "Your goal is to kill as many Enemies as you can and ", new Vector2(horizontal_margin, vertical_margin + font_height), font_color);
        spriteBatch.DrawString(font, "proceed to the Exit Point.", new Vector2(horizontal_margin, vertical_margin + 2 * font_height), font_color);
        

        
        int x_pos = 8 * horizontal_margin;
        int empty_space = 4;
        int slash_length = (int) font.MeasureString("/").X;

        spriteBatch.DrawString(font, "Player Movement : ", new Vector2(horizontal_margin, vertical_margin + 4 * font_height), font_color);

        int y_pos = vertical_margin + 4 * font_height;
        spriteBatch.Draw(Left_Stick, new Rectangle(x_pos, y_pos, font_height, font_height), Color.White);

        spriteBatch.DrawString(font, "(Left Stick) /", new Vector2(x_pos + font_height + empty_space, y_pos), font_color);
        int string_length = (int)font.MeasureString("(Left Stick) /").X;
        spriteBatch.Draw(WASD, new Rectangle(x_pos + font_height + 2 * empty_space + string_length, y_pos, font_height, font_height), Color.White);
        spriteBatch.Draw(Arrow_Keys, new Rectangle(x_pos + 2 * font_height + 3 * empty_space  + string_length, y_pos, font_height, font_height), Color.White);

        spriteBatch.DrawString(font, "Pull the Rope : ", new Vector2(horizontal_margin, vertical_margin + 5 * font_height), font_color);

        y_pos = vertical_margin + 5 * font_height;
        spriteBatch.Draw(RT, new Rectangle(x_pos, y_pos, font_height, font_height), Color.White);
        spriteBatch.DrawString(font, "/", new Vector2(x_pos + font_height + empty_space, y_pos), font_color);
        spriteBatch.Draw(P, new Rectangle(x_pos + font_height + 2 * empty_space + slash_length, y_pos, font_height, font_height), Color.White);
        

        spriteBatch.DrawString(font, "Dash : ", new Vector2(horizontal_margin, vertical_margin + 6 * font_height), font_color);

        y_pos = vertical_margin + 6 * font_height;
        spriteBatch.Draw(A, new Rectangle(x_pos, y_pos, font_height, font_height), Color.White);
        spriteBatch.DrawString(font, "/", new Vector2(x_pos + font_height + empty_space, y_pos), font_color);
        spriteBatch.Draw(Space, new Rectangle(x_pos + font_height + 2 * empty_space + slash_length, y_pos, font_height, font_height), Color.White);

        spriteBatch.DrawString(font, "Change between Spears : ", new Vector2(horizontal_margin, vertical_margin + 7 * font_height), font_color);

        y_pos = vertical_margin + 7 * font_height;
        spriteBatch.Draw(LB, new Rectangle(x_pos, y_pos, font_height, font_height), Color.White);
        spriteBatch.Draw(RB, new Rectangle(x_pos + font_height + empty_space, y_pos, font_height, font_height), Color.White);
        spriteBatch.DrawString(font, "/", new Vector2(x_pos + 2 * font_height + 2 * empty_space, y_pos), font_color);
        spriteBatch.Draw(Q, new Rectangle(x_pos + 2 * font_height + 3 * empty_space + slash_length, y_pos, font_height, font_height), Color.White);
        spriteBatch.Draw(E, new Rectangle(x_pos + 3 * font_height + 4 * empty_space + slash_length, y_pos, font_height, font_height), Color.White);

        spriteBatch.DrawString(font, "Place a Spear: ", new Vector2(horizontal_margin, vertical_margin + 8 * font_height), font_color);

        y_pos = vertical_margin + 8 * font_height;
        spriteBatch.Draw(X, new Rectangle(x_pos, y_pos, font_height, font_height), Color.White);
        spriteBatch.DrawString(font, "/", new Vector2(x_pos + font_height + empty_space, y_pos), font_color);
        spriteBatch.Draw(R, new Rectangle(x_pos + font_height + 2 * empty_space + slash_length, y_pos, font_height, font_height), Color.White);

        spriteBatch.DrawString(font, "Pause/Back to Menu: ", new Vector2(horizontal_margin, vertical_margin + 9 * font_height), font_color);
        spriteBatch.DrawString(font, "Start / Esc", new Vector2(8 * horizontal_margin, vertical_margin + 9 * font_height), font_color);

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        //
    }
    
}