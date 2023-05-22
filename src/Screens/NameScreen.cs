using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace TwistedDescent.Screens;

public class NameScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    
    private Texture2D _menu_img;
    private Texture2D _menu_title;

    private Texture2D _bg;
    private ContentManager _content;

    private Button letter1;
    private Button letter2;
    private Button letter3;
    public string playerName;
    private Button _activeButton;
    private Char[] letters = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

    private int w;
    private int h;

    public NameScreen(RopeGame game, ContentManager content) : base(game) {
        var buttonTexture = content.Load<Texture2D>("Sprites/UI/menu_selection_highlight");
        var buttonFont = content.Load<SpriteFont>("Fonts/damn");

        _content = content;

        _game.LoadSettings();

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _menu_img = content.Load<Texture2D>("Sprites/UI/menu_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");
        
        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        // The Buttons are aligned within a "Box" on the right half of the menu screen. 100 pixels margin to the right / bottom are left
        // The Boxes Height : Width Ratio is 2 : 3 (same as the logo). Computing the maximal size of the box:
        int font_width = (int)buttonFont.MeasureString("A").X;
        int start_width = (int)buttonFont.MeasureString("Start").X;

        Button go_next = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w - 2 * start_width, h / 16),
            Text = "Start"
        };

        go_next.Click += NewGameButton_Click;

        letter1 = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w / 2 - start_width, h / 7),
            Text = "A"
        };

        letter2 = new Button(buttonTexture, buttonFont) {
            Position = new Vector2(w / 2, h / 7),
            Text = "B"
        };

        letter3 = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w / 2 + 100, h / 7),
            Text = "C",
        };


        _components = new List<Component> {
            go_next,
            letter1,
            letter2,
            letter3,
        };
    }

    private void NewGameButton_Click(object sender, EventArgs e) {
        //base.getGame().ResetGame();
        base.getGame()._menuScreen._continueButton.Disabled = false;
        base.getGame()._loadingScreen = new LoadingScreen(base.getGame(), _content);
        base.getGame()._loadingScreen.Initialize();
        base.getGame()._transitionScreen = new TransitionScreen(base.getGame(), _content);
        base.getGame()._transitionScreen.Initialize();
        playerName = letter1.Text + "." + letter2.Text + "." + letter3.Text;
        base.getGame().ChangeState(RopeGame.State.Loading);
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

        //if (_activeButton != null && Input.IsButtonPressed(Buttons.A, true)) {
        //    _activeButton.Trigger();
        //}

        var buttons = _components.OfType<Button>().Where(button => !button.Disabled).ToList();
        if (Input.IsButtonPressed(Buttons.DPadRight, true) || Input.IsKeyPressed(Keys.Right, true)) {
            if (_activeButton == null) {
                _activeButton = buttons.First();
            } else {
                var next = ((buttons.IndexOf(_activeButton) + 1) % buttons.Count + buttons.Count) % buttons.Count;
                _activeButton = buttons[next];
            }
            buttons.ForEach(button => button.SetHover(false));
            _activeButton.SetHover(true);
        } else if (Input.IsButtonPressed(Buttons.DPadLeft, true) || Input.IsKeyPressed(Keys.Left, true)) {
            if (_activeButton == null) {
                _activeButton = buttons.First();
            } else {
                var next = ((buttons.IndexOf(_activeButton) - 1) % buttons.Count + buttons.Count) % buttons.Count;
                _activeButton = buttons[next];
            }
            
            buttons.ForEach(button => button.SetHover(false));
            _activeButton.SetHover(true);
        }
        else if (Input.IsButtonPressed(Buttons.DPadUp, true) || Input.IsKeyPressed(Keys.Up, true))
        {
            if (_activeButton != null && (buttons.IndexOf(_activeButton) != 0))
            {
                char l = char.Parse(_activeButton.Text);
                int next_char_ascii = (int)l + 1;
                if (next_char_ascii == 91)
                    next_char_ascii = 65;
                _activeButton.Text = ((char)next_char_ascii).ToString();
            }
        }
        else if (Input.IsButtonPressed(Buttons.DPadDown, true) || Input.IsKeyPressed(Keys.Down, true))
        {
            if (_activeButton != null && (buttons.IndexOf(_activeButton) != 0))
            {
                char l = char.Parse(_activeButton.Text);
                int next_char_ascii = (int)l - 1;
                if (next_char_ascii == 64)
                    next_char_ascii = 90;
                _activeButton.Text = ((char)next_char_ascii).ToString();
            }
        }
    }
}