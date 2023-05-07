using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Screens; 

public class TransitionScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    private Texture2D _playerModel;
    private Texture2D _enemyModel;

    private Texture2D _menu_img;
    private Texture2D _menu_title;
    private Texture2D _bg;

    public Boolean gameLoaded;
    public double timer;
    private SpriteFont font;

    private int w;
    private int h;

    public TransitionScreen(RopeGame game, ContentManager content) : base(game)
    {
        w = game.Graphics.PreferredBackBufferWidth;
        h = game.Graphics.PreferredBackBufferHeight;

        font = content.Load<SpriteFont>("Damn");
        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");
        _enemyModel = content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle_flip");

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _menu_img = content.Load<Texture2D>("Sprites/UI/menu_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");

        gameLoaded = false;
        timer = 0;

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
        //spriteBatch.Draw(_playerModel, new Rectangle(w / 4 * 3 - 10, h / 9, w / 4, h / 9 * 7), Color.White);
        //spriteBatch.Draw(_enemyModel, new Rectangle(10, h / 9, w / 4, h / 9 * 7), Color.White);
        spriteBatch.DrawString(font, "Generating The next Level", new Vector2(w / 2 - 270, h / 9 * 2), Color.White);
        spriteBatch.DrawString(font, "This Might Take Couple Of Seconds", new Vector2(w / 2 - 350, h / 9 * 3), Color.White);
        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        timer += gameTime.ElapsedGameTime.TotalMilliseconds;

        if (!gameLoaded && timer > 100)
        {
            gameLoaded = true;
            base.getGame()._gameScreen.LoadNextLevel();
            base.getGame().ChangeState(RopeGame.State.Running);
        }
    }
    
}