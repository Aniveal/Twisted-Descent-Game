using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Common;
using static System.Net.Mime.MediaTypeNames;

namespace TwistedDescent.Screens;

public class OptionsScreen : Screen
{
    private List<Component> _components;
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

    public override void Initialize()
    {
        base.Initialize();

        var buttonTexture = base.getGame().Content.Load<Texture2D>("Sprites/UI/menu_selection_highlight");
        var buttonFont = base.getGame().Content.Load<SpriteFont>("Fonts/damn");

        w = base.getGame().GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = base.getGame().GraphicsDevice.PresentationParameters.BackBufferHeight;

        // The Buttons are aligned within a "Box" on the right half of the menu screen. 100 pixels margin to the right / bottom are left
        // The Boxes Height : Width Ratio is 2 : 3 (same as the logo). Computing the maximal size of the box:
        int text_box_width = Math.Min(w / 2 - 150, 2 * (h - 200) / 3);
        int text_box_height = 2 * text_box_width / 3;

        text_box_height = Math.Max(text_box_height, 300); // Fixing overlapping 


        var backButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 0 * (text_box_height / 4)),
            Text = "Back"
        };

        backButton.Click += BackButton_Clicked;

        var fullscreenButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 1 * (text_box_height / 4)),
            Text = "Fullscreen"
        };
        if (base.getGame().isFullscreen) fullscreenButton.Text = "Windowed";

        fullscreenButton.Click += FullscreenButton_Clicked;

        var musicVolumeButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 4 * (text_box_height / 4)),
            Text = "Music Volume: " + SoundEngine.Instance.musicVolume.ToString("F1")
        };

        musicVolumeButton.Click += MusicVolume_Clicked;

        var effectVolumeButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 3 * (text_box_height / 4)),
            Text = "Effect Volume: " + SoundEngine.Instance.effectVolume.ToString("F1")
        };
        effectVolumeButton.Click += EffectVolume_Clicked;



        var resolutionButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 7 * (text_box_height / 4)),
            Text = _game.resolutionTexts[_game.resolutionIndex]
        };

        resolutionButton.Click += ResolutionButton_Clicked;

        var applyButton = new Button(buttonTexture, buttonFont)
        {
            Position = new Vector2(w - 100 - text_box_width, h - 120 - 6 * (text_box_height / 4)),
            Text = "Apply Resolution"
        };

        applyButton.Click += ApplyButton_Clicked;





        this._components = new List<Component> {
            backButton,
            fullscreenButton,
            effectVolumeButton,
            musicVolumeButton,
            resolutionButton,
            applyButton
        };
    }

    public OptionsScreen(RopeGame game, ContentManager content) : base(game)
    {
        _content = content;

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _menu_img = content.Load<Texture2D>("Sprites/UI/menu_img");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");

        Initialize();

    }

    private void ResolutionButton_Clicked(object sender, EventArgs e)
    {
        _game.resolutionIndex += 1;
        if (_game.resolutionIndex >= _game.resolutions.Count) _game.resolutionIndex = 0;
        Button b = sender as Button;
        b.Text = _game.resolutionTexts[_game.resolutionIndex];
    }

    private void ApplyButton_Clicked(Object sender, EventArgs e)
    {
        _game.ApplyResolution();
        _game.SaveSettings();
        Initialize();
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        _game.SaveSettings();

        base.getGame()._menuScreen = new MenuScreen(base.getGame(), base.getGame().GraphicsDevice, _content);
        base.getGame()._menuScreen.Initialize();
        base.getGame().ChangeState(RopeGame.State.MainMenu);
    }

    private void EffectVolume_Clicked(object sender, EventArgs e)
    {
        float vol = SoundEngine.Instance.effectVolume;
        vol += 0.1f;
        vol = (float)Math.Round(vol, 1);
        if (vol > 1f)
            vol = 0f;
        SoundEngine.Instance.SetEffectVolume(vol);
        Button s = sender as Button;
        s.Text = "Effect Volume: " + SoundEngine.Instance.effectVolume.ToString("F1");
    }

    private void MusicVolume_Clicked(object sender, EventArgs e)
    {
        float vol = SoundEngine.Instance.musicVolume;
        vol += 0.1f;
        vol = (float)Math.Round(vol, 1);
        if (vol > 1f)
            vol = 0f;
        SoundEngine.Instance.SetMusicVolume(vol);
        Button s = sender as Button;
        s.Text = "Music Volume: " + SoundEngine.Instance.musicVolume.ToString("F1");
    }

    private void FullscreenButton_Clicked(object sender, EventArgs e)
    {
        _game.isFullscreen = !_game.isFullscreen;
        _game.setFullscreen(_game.isFullscreen);
        Initialize();
    }


    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_bg, new Rectangle(0, 0, w, h), Color.White);

        // Img is drawn on the left half of the screen, we leave 100 pixels to the left / top / bottom and 50 to the middle
        // widht : height ratio of Img is 1 : 2
        int img_width = Math.Min(w / 2 - 150, (h - 200) / 2);
        int img_height = 2 * img_width;
        spriteBatch.Draw(_menu_img, new Rectangle(w - 100 - img_width, h - 100 - img_height, img_width, img_height), Color.White);

        foreach (var component in _components)
            component.Draw(gameTime, spriteBatch);

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var component in _components)
            component.Update(gameTime);

        if (_activeButton != null && Input.IsButtonPressed(Buttons.A, true))
        {
            _activeButton.Trigger();
        }

        var buttons = _components.OfType<Button>().Where(button => !button.Disabled).ToList();
        if (Input.IsButtonPressed(Buttons.DPadDown, true))
        {
            if (_activeButton == null)
            {
                _activeButton = buttons.First();
            }
            else
            {
                var next = ((buttons.IndexOf(_activeButton) + 1) % buttons.Count + buttons.Count) % buttons.Count;
                _activeButton = buttons[next];
            }

            buttons.ForEach(button => button.SetHover(false));
            _activeButton.SetHover(true);
        }
        else if (Input.IsButtonPressed(Buttons.DPadUp, true))
        {
            if (_activeButton == null)
            {
                _activeButton = buttons.First();
            }
            else
            {
                var next = ((buttons.IndexOf(_activeButton) - 1) % buttons.Count + buttons.Count) % buttons.Count;
                _activeButton = buttons[next];
            }

            buttons.ForEach(button => button.SetHover(false));
            _activeButton.SetHover(true);
        }
    }
}