using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 

public class MenuScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;

    public MenuScreen(RopeGame game, GraphicsDevice graphicsDevice, ContentManager content) : base(game) {
        
        var buttonTexture = content.Load<Texture2D>("Button");
        var buttonFont = content.Load<SpriteFont>("Arial");

        var newGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(700, 200),
            Text = "New Game"
        };

        newGameButton.Click += NewGameButton_Click;

        var loadGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(700, 250),
            Text = "HighScore"
        };

        loadGameButton.Click += HighScoreButton_Click;

        var quitGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(700, 300),
            Text = "Quit Game"
        };

        quitGameButton.Click += QuitGameButton_Click;

        _components = new List<Component> {
            newGameButton,
            loadGameButton,
            quitGameButton
        };
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        spriteBatch.Begin();

        foreach (var component in _components)
            component.Draw(gameTime, spriteBatch);

        spriteBatch.End();
    }

    private void HighScoreButton_Click(object sender, EventArgs e) {
        //Console.WriteLine("Load Game");
    }

    private void NewGameButton_Click(object sender, EventArgs e) {
        base.getGame().ChangeState(1);
    }

    public override void Update(GameTime gameTime) {
        foreach (var component in _components)
            component.Update(gameTime);
    }

    private void QuitGameButton_Click(object sender, EventArgs e) {
        base.getGame().Exit();
    }
}