using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwistedDescent.Screens; 

public class TransitionScreen : Screen {
    private readonly List<Component> _components;
    private SpriteBatch _spriteBatch;
    public GraphicsDeviceManager Graphics;
    private String _loadingText;
    private Texture2D _loading_img;
    private Texture2D _menu_title;
    private Texture2D _bg;

    public Boolean gameLoaded;
    public double timer;
    private SpriteFont font;
    private Color font_colour;

    private int w;
    private int h;

    public TransitionScreen(RopeGame game, ContentManager content) : base(game)
    {
        w = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        h = game.GraphicsDevice.PresentationParameters.BackBufferHeight;

        font = content.Load<SpriteFont>("Fonts/damn");

        _bg = content.Load<Texture2D>("Sprites/UI/menu_background");
        _loading_img = content.Load<Texture2D>("Sprites/UI/loading_img_cyclop");
        _menu_title = content.Load<Texture2D>("Sprites/UI/menu_title");

        gameLoaded = true;
        timer = 0;
        font_colour = new Color(154, 134, 129);
        _loadingText = "Loading ...";
    }
    
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.Draw(_bg, new Rectangle(0, 0, w, h), Color.White);
        int title_width = Math.Min(w / 2 - 150, 2 * (h - 200) / 3);
        int title_height = 2 * title_width / 3;
        spriteBatch.Draw(_menu_title, new Rectangle(100, h - title_height - 100, title_width, title_height), Color.White);
        int img_width = Math.Min(w / 2 - 150, (h - 200) / 2);
        int img_height = 2 * img_width;
        img_width = (int)(5 * img_width / 4);
        spriteBatch.Draw(_loading_img, new Rectangle(w - 100 - img_width, h - 100 - img_height, img_width, img_height), Color.White);

        
        Vector2 text_position = new Vector2(w - 100 - font.MeasureString(_loadingText).X, h - 85 - font.MeasureString(_loadingText).Y);
        spriteBatch.DrawString(font, _loadingText, text_position, font_colour);

        String text = "Level Complete! Proceed to Next Level" ;
        text_position = new Vector2(w / 16, 90 + font.MeasureString(text).Y);
        spriteBatch.DrawString(font, text, text_position, font_colour);

        spriteBatch.End();
    }

    public override void Update(GameTime gameTime) {
        Input.GetState();
        timer += gameTime.ElapsedGameTime.TotalMilliseconds;
        if (!gameLoaded && timer > 100)
        {
            gameLoaded = true;
            base.getGame()._gameScreen.LoadNextLevel();
            _loadingText = "Press Space/A  to Continue ...";
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Space) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
        {
        
            base.getGame().ChangeState(RopeGame.State.Running);
        }

    }
    
}