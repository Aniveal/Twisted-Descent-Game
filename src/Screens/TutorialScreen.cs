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
    public Boolean gameLoaded;
    double timer;
    private SpriteFont font;

    public TutorialScreen(RopeGame game, ContentManager content) : base(game)
    {
        font = content.Load<SpriteFont>("Arial40");
        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");
        _enemyModel = content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle_flip");
        _bg = content.Load<Texture2D>("Sprites/bg");
        gameLoaded = false;
        timer = 0;

    }
    
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_bg, new Rectangle(0, 0, 1600, 900), Color.White);

        spriteBatch.DrawString(font, "In this game you are playing as Theseus (", new Vector2(100, 50), Color.White);
        spriteBatch.Draw(_playerModel, new Rectangle(1065, 50, 60, 60), Color.White);
        spriteBatch.DrawString(font, ")", new Vector2(1120, 50), Color.White);

        spriteBatch.DrawString(font, "Your goal is to kill as many Enemies ( ", new Vector2(100, 100), Color.White);
        spriteBatch.Draw(_enemyModel, new Rectangle(970, 100, 60, 60), Color.White);
        spriteBatch.DrawString(font, ") as you can and ", new Vector2(1030, 100), Color.White);
        spriteBatch.DrawString(font, "proceed to the Exit Point ", new Vector2(100, 150), Color.White);


        spriteBatch.DrawString(font, "Player Movement : ", new Vector2(100, 300), Color.White);
        spriteBatch.DrawString(font, "Arrow Keys", new Vector2(1000, 300), Color.White);

        spriteBatch.DrawString(font, "Pull the Rope : ", new Vector2(100, 400), Color.White);
        spriteBatch.DrawString(font, "P", new Vector2(1000, 400), Color.White);

        spriteBatch.DrawString(font, "Dash : ", new Vector2(100, 500), Color.White);
        spriteBatch.DrawString(font, "Space", new Vector2(1000, 500), Color.White);

        spriteBatch.DrawString(font, "Change between Spears : ", new Vector2(100, 600), Color.White);
        spriteBatch.DrawString(font, "Q, E", new Vector2(1000, 600), Color.White);

        spriteBatch.DrawString(font, "Place a Spear: ", new Vector2(100, 700), Color.White);
        spriteBatch.DrawString(font, "R", new Vector2(1000, 700), Color.White);

        spriteBatch.DrawString(font, "Pause/Back to Menu: ", new Vector2(100, 800), Color.White);
        spriteBatch.DrawString(font, "Esc", new Vector2(1000, 800), Color.White);


        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        //
    }
    
}