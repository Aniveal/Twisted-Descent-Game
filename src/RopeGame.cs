using System;
using Meridian2.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2;

public class RopeGame : Game {
    private const int TargetFrameRate = 144;

    public MapScreen _mapScreen;
    public GameScreen _gameScreen;
    public MenuScreen _menuScreen;
    public TutorialScreen _tutorialScreen;
    public Screen _currentScreen;
    public LoadingScreen _loadingScreen;
    public FinalScreen _finalScreen;
    public TransitionScreen _transitionScreen;

    private SpriteBatch _spriteBatch;

    public Texture2D ColumnTexture;

    //TODO: move textures somewhere else?
    public SpriteFont Font;

    public GameData GameData;

    private State _currentState;
    private bool gameScreen = true;

    public GraphicsDeviceManager Graphics;
    private int currentWidth;
    private int currentHeight;

    bool isFullscreen = false;

    public enum State {
        Running,
        Pause,
        MainMenu,
        Loading,
        Tutorial,
        Transition,
        Final
    }

    public RopeGame() {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void ResetGame() {
        _gameScreen = null;
    }

    public void ChangeState(State state) {
        if (state == State.MainMenu) {
            _currentScreen = _menuScreen;
        } else if (state == State.Running) {
            if (gameScreen)
                _currentScreen = _gameScreen;
            else
                _currentScreen = _mapScreen;
        }
        else if (state == State.Transition)
        {
            _currentScreen = _transitionScreen;
        }
        else if (state == State.Loading)
        {
            _currentScreen = _loadingScreen;
        }
        else if (state == State.Tutorial)
        {
            _currentScreen = _tutorialScreen;
        }
        else if (state == State.Final)
        {
            _currentScreen = _finalScreen;
        }

        _currentState = state;
    }

    protected override void Initialize() {
        base.Initialize();

        // Set target framerate
        var temp = 1000d / TargetFrameRate * 10000d;
        TargetElapsedTime = new TimeSpan((long)temp);

        // Set resolution
        Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        Graphics.IsFullScreen = isFullscreen;
        Graphics.ApplyChanges();

        _menuScreen = new MenuScreen(this, GraphicsDevice, Content);
        _menuScreen.Initialize();
        _currentScreen = _menuScreen;
        _currentState = State.MainMenu;

        currentWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        currentHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        SoundEngine.Instance.SetRopeGame(this);
        SoundEngine.Instance.playTheme();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Font = Content.Load<SpriteFont>("Arial");
        ColumnTexture = Content.Load<Texture2D>("circle");
    }

    private void ToggleFullscreen() {
        if (Graphics.IsFullScreen) {
            Graphics.IsFullScreen = false;
            Graphics.HardwareModeSwitch = true;
        } else {
            Graphics.IsFullScreen = true;
            Graphics.HardwareModeSwitch = false;
        }

        Graphics.ApplyChanges();
    }
    
    protected override void Update(GameTime gameTime) {
        Input.GetState();
        if (Input.IsButtonPressed(Buttons.Back, true) || Input.IsKeyPressed(Keys.Escape, true) || (Input.IsButtonPressed(Buttons.B, true) && _currentState is State.Tutorial or State.Final)) {
            if (_currentState == State.MainMenu && _gameScreen != null) {
                ChangeState(State.Running);
            } else {
                ChangeState(State.MainMenu);
            }
        }

        if (Input.IsKeyPressed(Keys.F11, true)) {
            ToggleFullscreen();
        }

        // If the resolution changed, redraw main menu
        if (currentWidth != GraphicsDevice.PresentationParameters.BackBufferWidth || 
            currentHeight != GraphicsDevice.PresentationParameters.BackBufferHeight) {
            _menuScreen = new MenuScreen(this, GraphicsDevice, Content);
            _menuScreen.Initialize();
            
            ChangeState(_currentState);

            currentWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            currentHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }
        
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