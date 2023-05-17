using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwistedDescent.Screens;

namespace TwistedDescent;

public class RopeGame : Game {
    private const int TargetFrameRate = 144;

    public OptionsScreen _optionsScreen;
    public MapScreen _mapScreen;
    public GameScreen _gameScreen;
    public MenuScreen _menuScreen;
    public ControlScreen _contorlScreen;
    public Screen _currentScreen;
    public LoadingScreen _loadingScreen;
    public TutorialLoadingScreen _tutorialLoadingScreen;
    public FinalScreen _finalScreen;
    public TransitionScreen _transitionScreen;

    private SpriteBatch _spriteBatch;

    public Texture2D ColumnTexture;

    //TODO: move textures somewhere else?
    public SpriteFont Font;

    public GameData GameData;

    public int displayWidth;
    public int displayHeight;

    private State _currentState;
    private bool gameScreen = true;

    public GraphicsDeviceManager Graphics;
    private int currentWidth;
    private int currentHeight;

    public bool isFullscreen = false;
    public bool controller_connected = true;

    public enum State {
        Running,
        Pause,
        MainMenu,
        Loading,
        Tutorial,
        Controls,
        Transition,
        Final,
        Options
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
        else if (state == State.Tutorial) {
            _currentScreen = _tutorialLoadingScreen;
        }
        else if (state == State.Controls)
        {
            _currentScreen = _contorlScreen;
        }
        else if (state == State.Final)
        {
            _currentScreen = _finalScreen;
        }
        else if (state == State.Options)
        {
            _currentScreen = _optionsScreen;
        }

        _currentState = state;
    }

    public void setFullscreen(bool fullscreen)
    {
        isFullscreen = fullscreen;
        Graphics.PreferredBackBufferWidth = displayWidth;
        Graphics.PreferredBackBufferHeight = displayHeight;
        Graphics.IsFullScreen = isFullscreen;
        Graphics.ApplyChanges();
        this.Window.Position = new Point(50, 50);
    }

    protected override void Initialize() {
        base.Initialize();

        // Set target framerate
        var temp = 1000d / TargetFrameRate * 10000d;
        TargetElapsedTime = new TimeSpan((long)temp);

        // Set resolution
        displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        Graphics.PreferredBackBufferWidth = displayWidth;
        Graphics.PreferredBackBufferHeight = displayHeight;
        setFullscreen(isFullscreen);

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

        Font = Content.Load<SpriteFont>("Fonts/Arial");
        ColumnTexture = Content.Load<Texture2D>("circle");
    }

    private void ToggleFullscreen() {
        // if (Graphics.IsFullScreen) {
        //     Graphics.IsFullScreen = false;
        //     Graphics.HardwareModeSwitch = true;
        // } else {
        //     Graphics.IsFullScreen = true;
        //     Graphics.HardwareModeSwitch = false;
        // }
        //
        // Graphics.ApplyChanges();
    }
    
    protected override void Update(GameTime gameTime) {
        Input.GetState();
        if (Input.IsButtonPressed(Buttons.Back, true) || Input.IsKeyPressed(Keys.Escape, true) || (Input.IsButtonPressed(Buttons.B, true) && _currentState is State.Controls or State.Final)) {
            if (_currentState == State.MainMenu && _gameScreen != null) {
                ChangeState(State.Running);
            } else {
                ChangeState(State.MainMenu);
            }
        }

        if (Input.IsKeyPressed(Keys.F11, true)) {
            ToggleFullscreen();
        }

        // test if a controller is connected
        GamePadCapabilities gamePadCapabilities = GamePad.GetCapabilities(PlayerIndex.One);
        controller_connected = gamePadCapabilities.IsConnected;

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