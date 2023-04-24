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
    public Camera Camera;

    public SoundEngine soundEngine;

    public GameScreen _gameScreen;
    private MapScreen _mapScreen;

    public Screen currentScreen;

    public GameData gameData;
    private State _currentState;

    private int _state = 0;

    public void ChangeState(int state)
    {
        _state = state;
    }

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
        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.ApplyChanges();

        _currentState = new MenuState(this, GraphicsDevice, Content);
        gameData = new GameData(this);

        bool gameScreen = true;

        if (gameScreen)
        {
            _gameScreen = new GameScreen(this);
            _gameScreen.Initialize();
            currentScreen = _gameScreen;
        }
        else
        {
            _mapScreen = new MapScreen(this);
            _mapScreen.Initialize();

            currentScreen = _mapScreen;
        }

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
            _state = 0;

        if (_state == 0)
            _currentState.Update(gameTime);
        else
            currentScreen.Update(gameTime);

        base.Update(gameTime);

        
    }

    protected override void Draw(GameTime gameTime) {
        //Color background_color = new Color(82,84,81);
        Color background_color = new Color(33, 34, 34);
        GraphicsDevice.Clear(background_color);

        if (_state == 0)
            _currentState.Draw(gameTime, _spriteBatch);
        else
            currentScreen.Draw(gameTime);

        base.Draw(gameTime);

        
    }
}