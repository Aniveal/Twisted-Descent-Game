using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace Meridian2.Screens;

public class LoadingScreen : Screen
{
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    private Texture2D _playerModel;
    private Texture2D _enemyModel;

    private Texture2D _loading_img;
    private Texture2D _menu_title;
    private Texture2D _bg;

    public Boolean gameLoaded;
    double timer;
    private SpriteFont font;

    private int w;
    private int h;

    private Color font_colour;

    public LoadingScreen(RopeGame game, ContentManager content) : base(game)
    {
        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        font = content.Load<SpriteFont>("Damn");
        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");
        _enemyModel = content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle_flip");

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _loading_img = content.Load<Texture2D>("Sprites/UI/loading_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");

        gameLoaded = false;
        timer = 0;


        font_colour = new Color(154, 134, 129);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_bg, new Rectangle(0, 0, w, h), Color.White);
        int title_width = Math.Min(w / 2 - 150, 2 * (h - 200) / 3);
        int title_height = 2 * title_width / 3;
        spriteBatch.Draw(_menu_title, new Rectangle(100, h - title_height - 100, title_width, title_height), Color.White);
        int img_width = Math.Min(w / 2 - 150, (h - 200) / 2);
        int img_height = 2 * img_width;
        spriteBatch.Draw(_loading_img, new Rectangle(w - 100 - img_width, h - 100 - img_height, img_width, img_height), Color.White);

        //spriteBatch.DrawString(font, "Generating The First Level", new Vector2(w / 2 - 270, h / 9 * 2), font_colour);
        //spriteBatch.DrawString(font, "This Might Take Couple Of Seconds", new Vector2(w / 2 - 350, h / 9 * 3), font_colour);

        String text = "Loading ...";
        Vector2 text_position = new Vector2(w - 100 - font.MeasureString(text).X, h - 85 - font.MeasureString(text).Y);
        spriteBatch.DrawString(font, text, text_position, font_colour);

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        timer += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (!gameLoaded && timer > 100)
        {
            gameLoaded = true;
            base.getGame().GameData = new GameData(base.getGame());
            base.getGame()._gameScreen = new GameScreen(base.getGame());
            base.getGame()._mapScreen = new MapScreen(base.getGame());
            base.getGame()._gameScreen.Initialize();
            base.getGame()._mapScreen.Initialize();
            base.getGame().ChangeState(RopeGame.State.Running);
        }
    }

}