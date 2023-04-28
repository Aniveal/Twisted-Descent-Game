using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Screens; 

public class LoadingScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    private Texture2D _playerModel;
    private Texture2D _enemyModel;
    private Texture2D _bg;
    public Boolean gameLoaded;
    double timer;
    private SpriteFont font;

    public LoadingScreen(RopeGame game, ContentManager content) : base(game)
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
        spriteBatch.Draw(_playerModel, new Rectangle(1190, 100, 400, 700), Color.White);
        spriteBatch.Draw(_enemyModel, new Rectangle(10, 100, 400, 700), Color.White);
        spriteBatch.DrawString(font, "Generating The Level", new Vector2(560, 200), Color.White);
        spriteBatch.DrawString(font, "This Might Take Couple Of Seconds", new Vector2(400, 320), Color.White);
        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
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