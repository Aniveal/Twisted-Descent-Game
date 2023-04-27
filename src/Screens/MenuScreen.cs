using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Screens;

public class MenuScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    private Texture2D _playerModel;
    private Texture2D _enemyModel;
    private Texture2D _bg;
    private Button _continueButton;

    public MenuScreen(RopeGame game, GraphicsDevice graphicsDevice, ContentManager content) : base(game) {
        var buttonTexture = content.Load<Texture2D>("Button");
        var buttonFont = content.Load<SpriteFont>("Arial40");
        _playerModel = content.Load<Texture2D>("Sprites/Theseus/model");
        _enemyModel = content.Load<Texture2D>("Sprites/Enemies/Minotaur/minotaur_idle");
        _bg = content.Load<Texture2D>("Sprites/BG");

        _continueButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(80, 50),
            Text = "Continue",
            Disabled = true
        };

        _continueButton.Click += ContinueButton_Click;

        var newGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(80, 250),
            Text = "New Game"
        };

        newGameButton.Click += NewGameButton_Click;

        var loadGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(80, 450),
            Text = "HighScore"
        };

        loadGameButton.Click += HighScoreButton_Click;

        var quitGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(80, 650),
            Text = "Quit Game"
        };

        quitGameButton.Click += QuitGameButton_Click;

        _components = new List<Component> {
            _continueButton,
            newGameButton,
            loadGameButton,
            quitGameButton
        };
    }

    private void ContinueButton_Click(object sender, EventArgs e) {
        if (base.getGame().GameScreen != null) {
            base.getGame().ChangeState(RopeGame.State.Running);
        }
    }

    private void NewGameButton_Click(object sender, EventArgs e) {
        base.getGame().ResetGame();
        base.getGame().ChangeState(RopeGame.State.Running);

        _continueButton.Disabled = false;
    }

    private void HighScoreButton_Click(object sender, EventArgs e) {
        //
    }

    private void QuitGameButton_Click(object sender, EventArgs e) {
        base.getGame().Exit();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Begin();
        spriteBatch.Draw(_bg, new Rectangle(0, 0, 1600, 900), Color.White);
        spriteBatch.Draw(_playerModel, new Rectangle(1150, 100, 400, 700), Color.White);
        spriteBatch.Draw(_enemyModel, new Rectangle(800, 100, 400, 700), Color.White);

        foreach (var component in _components)
            component.Draw(gameTime, spriteBatch);

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        foreach (var component in _components)
            component.Update(gameTime);
    }
}