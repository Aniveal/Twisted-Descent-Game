using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class RopeGame : Game {
    public GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int TargetFrameRate = 144;

    //TODO: move textures somewhere else?
    public SpriteFont Font;
    public Texture2D ColumnTexture;
    public Texture2D rectangleTexture;

    public SoundEngine soundEngine;

    private GameScreen _gameScreen;
    private MapScreen _mapScreen;

    private Screen currentScreen;

    public GameData gameData;

    public RopeGame() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize() {
        base.Initialize();

        // Set target framerate
        double temp = (1000d / TargetFrameRate) * 10000d;
        TargetElapsedTime = new TimeSpan((long)temp);

        // Set resolution
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();

        gameData = new GameData();

        _gameScreen = new GameScreen(this);
        _gameScreen.Initialize();

        _mapScreen = new MapScreen(this);
        _mapScreen.Initialize();

        currentScreen = _gameScreen;

        soundEngine = new SoundEngine(this); //Create the sound engine

    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Font = Content.Load<SpriteFont>("Arial");
        ColumnTexture = Content.Load<Texture2D>("circle");
        rectangleTexture = Content.Load<Texture2D>("rectangle");
    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);

        currentScreen.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);

        currentScreen.Draw(gameTime);
    }
}