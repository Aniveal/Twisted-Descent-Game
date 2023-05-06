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
    
    private Texture2D _menu_img;
    private Texture2D _menu_title;

    private Texture2D _bg;
    private Button _continueButton;
    private ContentManager _content;

    private int w;
    private int h;

    public MenuScreen(RopeGame game, GraphicsDevice graphicsDevice, ContentManager content) : base(game) {
        var buttonTexture = content.Load<Texture2D>("Sprites/UI/menu_selection_highlight");
        var buttonFont = content.Load<SpriteFont>("Damn");

        _content = content;

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _menu_img = content.Load<Texture2D>("Sprites/UI/menu_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");

        w = game.Graphics.PreferredBackBufferWidth;
        h = game.Graphics.PreferredBackBufferHeight;

        _continueButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(9 * w / 16, 4 * h / 9),
            Text = "Continue",
            Disabled = true
        };

        _continueButton.Click += ContinueButton_Click;

        var newGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(9 * w / 16, 5 * h / 9),
            Text = "New Game"
        };

        newGameButton.Click += NewGameButton_Click;

        var tutorialButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(9 * w / 16, 6 * h / 9),
            Text = "Tutorial",
        };

        tutorialButton.Click += TutorialButton_Click;

        var quitGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(9 * w / 16, 7 * h / 9),
            Text = "Quit Game"
        };

        quitGameButton.Click += QuitGameButton_Click;

        _components = new List<Component> {
            _continueButton,
            newGameButton,
            tutorialButton,
            quitGameButton
        };
    }

    private void ContinueButton_Click(object sender, EventArgs e) {
        if (base.getGame()._gameScreen != null) {
            base.getGame().ChangeState(RopeGame.State.Running);
        }
    }

    private void NewGameButton_Click(object sender, EventArgs e) {
        base.getGame().ResetGame();
        base.getGame()._loadingScreen = new LoadingScreen(base.getGame(), _content);
        base.getGame()._loadingScreen.Initialize();
        base.getGame().ChangeState(RopeGame.State.Loading);

        _continueButton.Disabled = false;
    }

    private void TutorialButton_Click(object sender, EventArgs e) {
        base.getGame()._tutorialScreen = new TutorialScreen(base.getGame(), _content);
        base.getGame()._tutorialScreen.Initialize();
        base.getGame().ChangeState(RopeGame.State.Tutorial);

    }

    private void QuitGameButton_Click(object sender, EventArgs e) {
        base.getGame().Exit();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Begin();
        spriteBatch.Draw(_bg, new Rectangle(0, 0, w, h), Color.White);
        spriteBatch.Draw(_menu_title, new Rectangle(w / 16, 4 * h / 9, 6 * w / 16, 4 * h / 9), Color.White);
        spriteBatch.Draw(_menu_img, new Rectangle(11 * w / 16, h / 9, 35 * w / 160, 7 * h / 9), Color.White);

        foreach (var component in _components)
            component.Draw(gameTime, spriteBatch);

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        foreach (var component in _components)
            component.Update(gameTime);
    }
}