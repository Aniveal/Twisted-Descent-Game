using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2;

public class RopeGame : Game {
    private const int TargetFrameRate = 144;
    private State _currentState;
    private MapScreen _mapScreen;
    private SpriteBatch _spriteBatch;

    private int _state;
    public Camera Camera;
    public Texture2D ColumnTexture;

    public Screen CurrentScreen;

    //TODO: move textures somewhere else?
    public SpriteFont Font;

    public GameData GameData;

    public GameScreen GameScreen;
    public GraphicsDeviceManager Graphics;
    public Texture2D RectangleTexture;

    public SoundEngine SoundEngine;

    public RopeGame() {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void ChangeState(int state) {
        _state = state;
    }

    protected override void Initialize() {
        base.Initialize();

        // Set target framerate
        var temp = 1000d / TargetFrameRate * 10000d;
        TargetElapsedTime = new TimeSpan((long)temp);

        // Set resolution
        Graphics.PreferredBackBufferWidth = 1600;
        Graphics.PreferredBackBufferHeight = 900;
        Graphics.ApplyChanges();

        _currentState = new MenuState(this, GraphicsDevice, Content);
        GameData = new GameData(this);

        var gameScreen = true;

        if (gameScreen) {
            GameScreen = new GameScreen(this);
            GameScreen.Initialize();
            CurrentScreen = GameScreen;
        } else {
            _mapScreen = new MapScreen(this);
            _mapScreen.Initialize();

            CurrentScreen = _mapScreen;
        }

        SoundEngine = new SoundEngine(this); //Create the sound engine
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Font = Content.Load<SpriteFont>("Arial");
        ColumnTexture = Content.Load<Texture2D>("circle");
        RectangleTexture = Content.Load<Texture2D>("rectangle");
    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            _state = 0;

        if (_state == 0)
            _currentState.Update(gameTime);
        else
            CurrentScreen.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        //Color background_color = new Color(82,84,81);
        var backgroundColor = new Color(33, 34, 34);
        GraphicsDevice.Clear(backgroundColor);

        if (_state == 0)
            _currentState.Draw(gameTime, _spriteBatch);
        else
            CurrentScreen.Draw(gameTime);

        base.Draw(gameTime);
    }
}