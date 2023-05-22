using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D;
using TwistedDescent.Screens;

namespace TwistedDescent;

public class RopeGame : Game {

    public Dictionary<string, int> leaderBoard = new Dictionary<string, int>();
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
    public HighScoreScreen _highScoreScreen;
    public NameScreen _nameScreen;

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

    public bool isFullscreen;
    public int resolutionIndex; //refers to the different resolutions in GameData
    public List<Point> resolutions;
    public List<string> resolutionTexts;

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
        Options,
        HighScore, 
        NameScreen
    }

    public RopeGame() {
        if (File.Exists("leaderBoard.txt")) {
            var lines = File.ReadLines("leaderBoard.txt");
            foreach (var line in lines)
            {
                string[] arr = line.Split(',');
                leaderBoard.Add(arr[0], Int16.Parse(String.Join(",", arr.Skip(1))));
            } 
        }
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        resolutions = new List<Point>()
        {
            new Point(1280, 720), //0
            new Point(1280, 1024), //1
            new Point(1440, 900), //2
            new Point(1600, 900), //3
            new Point(1600, 1200), //4
            new Point(1920, 1080), //5
            new Point(2560, 1080), //6 
            new Point(2560, 1440), //7
            new Point(2560, 1600), //8
            new Point(3840, 2160) //9
        };

        resolutionTexts = new List<string>()
        {
            "1280 x 720",
            "1280 x 1024",
            "1440 x 900",
            "1600 x 900",
            "1600 x 1200",
            "1920 x 1080",
            "2560 x 1080",
            "2560 x 1440",
            "2560 x 1600",
            "3048 x 2160"
        };
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
        else if (state == State.HighScore)
        {
            _currentScreen = _highScoreScreen;
        }
        else if (state == State.NameScreen)
        {
            _currentScreen = _nameScreen;
        }

        _currentState = state;
    }

    public void setFullscreen(bool fullscreen)
    {
        isFullscreen = fullscreen;
        Graphics.PreferredBackBufferWidth = resolutions[resolutionIndex].X;
        Graphics.PreferredBackBufferHeight = resolutions[resolutionIndex].Y;
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
        

        _menuScreen = new MenuScreen(this, GraphicsDevice, Content);
        _menuScreen.Initialize();
        _currentScreen = _menuScreen;
        _currentState = State.MainMenu;

        currentWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        currentHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        setFullscreen(isFullscreen);

        SoundEngine.Instance.SetRopeGame(this);
        SoundEngine.Instance.playTheme();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Font = Content.Load<SpriteFont>("Fonts/Arial");
        ColumnTexture = Content.Load<Texture2D>("circle");
    }

    public void LoadSettings()
    {
        string dir = Directory.GetCurrentDirectory() + "/settings.ini";

        try
        {
            using (StreamReader settings = File.OpenText(dir))
            {
                resolutionIndex = int.Parse(settings.ReadLine());
                setFullscreen(bool.Parse(settings.ReadLine()));
                SoundEngine.Instance.SetEffectVolume(float.Parse(settings.ReadLine()));
                SoundEngine.Instance.SetMusicVolume(float.Parse(settings.ReadLine()));
            }
        }

        catch
        {
            resolutionIndex = 0;
            setFullscreen(false);
            SoundEngine.Instance.SetEffectVolume(1f);
            SoundEngine.Instance.SetMusicVolume(1f);
            return;
        }
    }

    public void SaveSettings()
    {
        string dir = Directory.GetCurrentDirectory() + "/settings.ini";

        using (StreamWriter settings = File.CreateText(dir))
        {
            settings.WriteLine(resolutionIndex);
            settings.WriteLine(isFullscreen);
            settings.WriteLine(SoundEngine.Instance.effectVolume);
            settings.WriteLine(SoundEngine.Instance.musicVolume);
        }
    }



    private void ToggleFullscreen() 
    {
        isFullscreen = !isFullscreen;
        setFullscreen(isFullscreen);
    }

    public void ApplyResolution()
    {
        setFullscreen(isFullscreen);
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