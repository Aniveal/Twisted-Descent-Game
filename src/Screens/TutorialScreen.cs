using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Screens; 

public class TutorialScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    private Texture2D _playerModel;
    private Texture2D _enemyModel;

    private Texture2D _bg;
    private Texture2D _menu_img;
    private Texture2D _menu_title;

    public Boolean gameLoaded;
    double timer;
    private SpriteFont font;

    private int w;
    private int h;

    public TutorialScreen(RopeGame game, ContentManager content) : base(game)
    {
        font = content.Load<SpriteFont>("Arial40");
        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");
        _enemyModel = content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle_flip");

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _menu_img = content.Load<Texture2D>("Sprites/UI/menu_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");

        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        gameLoaded = false;
        timer = 0;

    }
    
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        spriteBatch.Draw(_bg, new Rectangle(0, 0, w, h), Color.White);
        int title_width = Math.Min(w / 2 - 150, 2 * (h - 200) / 3);
        int title_height = 2 * title_width / 3;
        //spriteBatch.Draw(_menu_title, new Rectangle(100, h - title_height - 100, title_width, title_height), Color.White);
        int img_width = Math.Min(w / 2 - 150, (h - 200) / 2);
        int img_height = 2 * img_width;
        //spriteBatch.Draw(_menu_img, new Rectangle(w - 100 - img_width, h - 100 - img_height, img_width, img_height), Color.White);

        spriteBatch.DrawString(font, "In this game you are playing as Theseus (", new Vector2(w / 16, h / 18), Color.White);
        spriteBatch.Draw(_playerModel, new Rectangle(w / 16 + 965, h / 18, w / 25, w / 25), Color.White);
        spriteBatch.DrawString(font, ")", new Vector2(w / 16 + 1020, h / 18), Color.White);

        spriteBatch.DrawString(font, "Your goal is to kill as many Enemies as you can and ", new Vector2(w / 16, h / 9), Color.White);
        spriteBatch.DrawString(font, "proceed to the Exit Point ", new Vector2(w / 16, h / 6), Color.White);


        spriteBatch.DrawString(font, "Player Movement : ", new Vector2(w / 16, h / 9 * 3), Color.White);
        spriteBatch.DrawString(font, "Arrows/ WASD", new Vector2(w / 16 * 12, h / 9 * 3), Color.White);

        spriteBatch.DrawString(font, "Pull the Rope : ", new Vector2(w / 16, h / 9 * 4), Color.White);
        spriteBatch.DrawString(font, "P", new Vector2(w / 16 * 12, h / 9 * 4), Color.White);

        spriteBatch.DrawString(font, "Dash : ", new Vector2(w / 16, h / 9 * 5), Color.White);
        spriteBatch.DrawString(font, "Space", new Vector2(w / 16 * 12, h / 9 * 5), Color.White);

        spriteBatch.DrawString(font, "Change between Spears : ", new Vector2(w / 16, h / 9 * 6), Color.White);
        spriteBatch.DrawString(font, "Q, E", new Vector2(w / 16 * 12, h / 9 * 6), Color.White);

        spriteBatch.DrawString(font, "Place a Spear: ", new Vector2(w / 16, h / 9 * 7), Color.White);
        spriteBatch.DrawString(font, "R", new Vector2(w / 16 * 12, h / 9 * 7), Color.White);

        spriteBatch.DrawString(font, "Pause/Back to Menu: ", new Vector2(w / 16, h / 9 * 8), Color.White);
        spriteBatch.DrawString(font, "Esc", new Vector2(w / 16 * 12, h / 9 * 8), Color.White);


        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        //
    }
    
}