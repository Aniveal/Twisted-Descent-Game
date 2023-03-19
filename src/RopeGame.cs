using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;

namespace Meridian2;

public class RopeGame : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private const int TargetFrameRate = 144;
    public SpriteFont Font;

    public GameScreen gameScreen;

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

        gameScreen = new GameScreen(this);
        gameScreen.Init();
    }

    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Font = Content.Load<SpriteFont>("Arial");
    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);

        gameScreen.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
        
        gameScreen.Draw();
    }
}