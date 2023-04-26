﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2;

public class RopeGame : Game {
    private const int TargetFrameRate = 144;

    private MapScreen _mapScreen;
    public GameScreen _gameScreen;
    private Screen _menuScreen;
    public Screen _currentScreen;

    private SpriteBatch _spriteBatch;

    private int _state;
    public Camera Camera;
    public Texture2D ColumnTexture;

    

    //TODO: move textures somewhere else?
    public SpriteFont Font;

    public GameData GameData;

    
    public GraphicsDeviceManager Graphics;
    public Texture2D RectangleTexture;

    public SoundEngine SoundEngine;

    public RopeGame() {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void ChangeState(int state) {
        if (state == 0)
        {
            _currentScreen = _menuScreen;
        }
        else if (state == 1)
        {
            var gameScreen = true;
            if (gameScreen)
                _currentScreen = _gameScreen;
            else
                _currentScreen = _mapScreen;
        }
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

        GameData = new GameData(this);

        _menuScreen = new MenuScreen(this, GraphicsDevice, Content);
        _menuScreen.Initialize();
        _gameScreen = new GameScreen(this);
        _gameScreen.Initialize();
        _mapScreen = new MapScreen(this);
        _mapScreen.Initialize();

        _currentScreen = _menuScreen;

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
            this.ChangeState(0);

        _currentScreen.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        //Color background_color = new Color(82,84,81);
        var backgroundColor = new Color(33, 34, 34);
        GraphicsDevice.Clear(backgroundColor);

        _currentScreen.Draw(gameTime, _spriteBatch);
        base.Draw(gameTime);
    }
}