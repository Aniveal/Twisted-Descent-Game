﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

    private Button _activeButton;

    private int w;
    private int h;

    public MenuScreen(RopeGame game, GraphicsDevice graphicsDevice, ContentManager content) : base(game) {
        var buttonTexture = content.Load<Texture2D>("Sprites/UI/menu_selection_highlight");
        var buttonFont = content.Load<SpriteFont>("damn");

        _content = content;

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _menu_img = content.Load<Texture2D>("Sprites/UI/menu_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");
        
        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        // The Buttons are aligned within a "Box" on the right half of the menu screen. 100 pixels margin to the right / bottom are left
        // The Boxes Height : Width Ratio is 2 : 3 (same as the logo). Computing the maximal size of the box:
        int text_box_width = Math.Min(w / 2 - 150, 2 * (h - 200) / 3);
        int text_box_height = 2 * text_box_width / 3;

        text_box_height = Math.Max(text_box_height, 300); // Fixing overlapping 

        _continueButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 4 * (text_box_height / 4)),
            Text = "Continue",
            Disabled = true
        };

        _continueButton.Click += ContinueButton_Click;

        var newGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 3 * (text_box_height / 4)),
            Text = "New Game"
        };

        newGameButton.Click += NewGameButton_Click;

        var tutorialButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 2 * (text_box_height / 4)),
            Text = "Controls",
        };

        tutorialButton.Click += TutorialButton_Click;

        var quitGameButton = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 1 * (text_box_height / 4)),
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
        base.getGame()._transitionScreen = new TransitionScreen(base.getGame(), _content);
        base.getGame()._transitionScreen.Initialize();
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

        // Title is drawn on the left half of the screen, we leave 100 pixels to the left / top / bottom and 50 to the middle
        // widht : height ratio of Title is 3 : 2
        int title_width = Math.Min(w / 2 - 150, 2 * (h - 200) / 3); 
        int title_height = 2 * title_width / 3;
        spriteBatch.Draw(_menu_title, new Rectangle(100, h - title_height - 100, title_width, title_height), Color.White);

        // Img is drawn on the left half of the screen, we leave 100 pixels to the left / top / bottom and 50 to the middle
        // widht : height ratio of Img is 1 : 2
        int img_width = Math.Min(w / 2 - 150,  (h - 200) / 2);
        int img_height = 2 * img_width;
        spriteBatch.Draw(_menu_img, new Rectangle(w - 100 - img_width, h - 100 - img_height, img_width, img_height), Color.White);

        foreach (var component in _components)
            component.Draw(gameTime, spriteBatch);

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        foreach (var component in _components)
            component.Update(gameTime);

        if (_activeButton != null && Input.IsButtonPressed(Buttons.A, true)) {
            _activeButton.Trigger();
        }

        var buttons = _components.OfType<Button>().Where(button => !button.Disabled).ToList();
        if (Input.IsButtonPressed(Buttons.DPadDown, true)) {
            if (_activeButton == null) {
                _activeButton = buttons.First();
            } else {
                var next = ((buttons.IndexOf(_activeButton) + 1) % buttons.Count + buttons.Count) % buttons.Count;
                _activeButton = buttons[next];
            }
            
            buttons.ForEach(button => button.SetHover(false));
            _activeButton.SetHover(true);
        } else if (Input.IsButtonPressed(Buttons.DPadUp, true)) {
            if (_activeButton == null) {
                _activeButton = buttons.First();
            } else {
                var next = ((buttons.IndexOf(_activeButton) - 1) % buttons.Count + buttons.Count) % buttons.Count;
                _activeButton = buttons[next];
            }
            
            buttons.ForEach(button => button.SetHover(false));
            _activeButton.SetHover(true);
        }
    }
}