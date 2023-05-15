using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwistedDescent.Screens; 

public class FinalScreen : Screen {
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
    private bool _timedOut;

    public FinalScreen(RopeGame game, ContentManager content, bool timedOut) : base(game)
    {
        font = content.Load<SpriteFont>("Fonts/Arial40");
        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");
        _enemyModel = content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle_flip");

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _menu_img = content.Load<Texture2D>("Sprites/UI/menu_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");

        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
        gameLoaded = false;
        timer = 0;
        _timedOut = timedOut;

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
        spriteBatch.Draw(_menu_img, new Rectangle(w - 100 - img_width, h - 100 - img_height, img_width, img_height), Color.White);
        if (_timedOut) {
            spriteBatch.DrawString(font, "Your time ran out!", new Vector2(w / 16, h / 9 * 2), Color.White);
        } else {
            spriteBatch.DrawString(font, "You Died!", new Vector2(w / 16, h / 9 * 2), Color.White);
        }
        spriteBatch.DrawString(font, "During the run, you killed " + (this.getGame().GameData.Kills) + " Enemies. Good Job!", new Vector2(w / 16, h / 9 * 3), Color.White);
        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        //
    }
    
}