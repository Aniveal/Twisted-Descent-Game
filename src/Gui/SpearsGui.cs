using Meridian2.Columns;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2.Gui; 

public class SpearsGui : DrawableGameElement {

    private readonly SpearsController _controller;
    private readonly GameData _data;
    private readonly RopeGame _game;

    private const int margin = 10;      // number of pixels distance to screen border / other elements
    private const int icon_size = 90;

    private const int gap_size = -20;
    private const int dash_ui_height = 80;

    private Color active_color = Color.White;
    private Color passive_color = new Color(100, 100, 100, 150);

    private Color active_font_color = new Color(191, 161, 154);
    private Color passive_font_color = new Color(111, 94, 90);

    private SpriteFont _font;

    private Texture2D _spears_background;

    private Texture2D _electric_spears_1;
    private Texture2D _electric_spears_2;
    private Texture2D _electric_spears_3;
    private Texture2D _metal_spears_1;
    private Texture2D _metal_spears_2;
    private Texture2D _metal_spears_3;
    private Texture2D _wooden_spears_1;
    private Texture2D _wooden_spears_2;
    private Texture2D _wooden_spears_3;
    
    private Texture2D _controls_lb;
    private Texture2D _controls_rb;
    private Texture2D _controls_x;
    private Texture2D _controls_x_pressed;


    public SpearsGui(RopeGame game, GameData data, SpearsController controller) {
        _game = game;
        _data = data;
        _controller = controller;
    }

    public void LoadContent() {
        _spears_background = _game.Content.Load<Texture2D>("Sprites/UI/spears_background");

        _electric_spears_1 = _game.Content.Load<Texture2D>("Sprites/UI/electric_spears_1");
        _electric_spears_2 = _game.Content.Load<Texture2D>("Sprites/UI/electric_spears_2");
        _electric_spears_3 = _game.Content.Load<Texture2D>("Sprites/UI/electric_spears_3");
        _metal_spears_1 = _game.Content.Load<Texture2D>("Sprites/UI/metal_spears_1");
        _metal_spears_2 = _game.Content.Load<Texture2D>("Sprites/UI/metal_spears_2");
        _metal_spears_3 = _game.Content.Load<Texture2D>("Sprites/UI/metal_spears_3");
        _wooden_spears_1 = _game.Content.Load<Texture2D>("Sprites/UI/wooden_spears_1");
        _wooden_spears_2 = _game.Content.Load<Texture2D>("Sprites/UI/wooden_spears_2");
        _wooden_spears_3 = _game.Content.Load<Texture2D>("Sprites/UI/wooden_spears_3");
        
        _controls_lb = _game.Content.Load<Texture2D>("Sprites/Controller/Buttons/Shoulder/button_xbox_digital_bumper_dark_1");
        _controls_rb = _game.Content.Load<Texture2D>("Sprites/Controller/Buttons/Shoulder/button_xbox_digital_bumper_dark_2");
        _controls_x = _game.Content.Load<Texture2D>("Sprites/Controller/Buttons/ABXY/button_xbox_digital_x_1");
        _controls_x_pressed = _game.Content.Load<Texture2D>("Sprites/Controller/Buttons/ABXY/button_xbox_digital_x_2");

        _font = _game.Content.Load<SpriteFont>("damn_ui");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera) {
        // computing positions
        var viewportHeight = _game.GraphicsDevice.Viewport.Height;
        var spearsUIHeight = viewportHeight - 2 * margin - dash_ui_height - icon_size;

        var pos_1 = new Rectangle(margin, spearsUIHeight, icon_size, icon_size);
        var pos_2 = new Rectangle(margin + icon_size + gap_size, spearsUIHeight, icon_size, icon_size);
        var pos_3 = new Rectangle(margin + 2 * (icon_size + gap_size), spearsUIHeight, icon_size, icon_size);


        // select sprite with 3, 2, or 1 spear on it, depending on how many are available to player
        var wooden_sprite = _wooden_spears_3;
        if (_data.Spears[2] == 2)
            wooden_sprite = _wooden_spears_2;
        else if (_data.Spears[2] <= 1)
            wooden_sprite = _wooden_spears_1;

        var electric_sprite = _electric_spears_3;
        if (_data.Spears[1] == 2)
            electric_sprite = _electric_spears_2;
        else if (_data.Spears[1] <= 1)
            electric_sprite = _electric_spears_1;

        var metal_sprite = _metal_spears_3;
        if (_data.Spears[0] == 2)
            metal_sprite = _metal_spears_2;
        else if (_data.Spears[0] <= 1)
            metal_sprite = _metal_spears_1;

        // making sure the count of spears is two digits (like 02 or 07)
        var wooden_spears_text = _data.Spears[2].ToString();
        if (wooden_spears_text.Length <= 1)
            wooden_spears_text = " " + wooden_spears_text;

        var electric_spears_text = _data.Spears[1].ToString();
        if (electric_spears_text.Length <= 1)
            electric_spears_text = " " + electric_spears_text;

        var metal_spears_text = _data.Spears[0].ToString();
        if (metal_spears_text.Length <= 1)
            metal_spears_text = " " + metal_spears_text;


        if (_controller.Selected == 1)  // Electric Spears at position 3 (right)
        {
            batch.Draw(_spears_background, pos_3, null, active_color, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
            batch.Draw(electric_sprite, pos_3, null, active_color, 0f, Vector2.Zero, SpriteEffects.None, 0.995f);
            batch.DrawString(_font, electric_spears_text, new Vector2(pos_3.X, pos_3.Y), active_font_color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
        else
        {
            batch.Draw(_spears_background, pos_3, null, passive_color, 0f, Vector2.Zero, SpriteEffects.None, 0.97f);
            batch.Draw(electric_sprite, pos_3, null, passive_color, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
            batch.DrawString(_font, electric_spears_text, new Vector2(pos_3.X, pos_3.Y), passive_font_color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.985f);
        }

        if (_controller.Selected == 0) // Metal Spears at position 2 (middle)
        {
            batch.Draw(_spears_background, pos_2, null, active_color, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
            batch.Draw(metal_sprite, pos_2, null, active_color, 0f, Vector2.Zero, SpriteEffects.None, 0.995f);
            batch.DrawString(_font, metal_spears_text, new Vector2(pos_2.X, pos_2.Y), active_font_color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
        else
        {
            batch.Draw(_spears_background, pos_2, null, passive_color, 0f, Vector2.Zero, SpriteEffects.None, 0.975f);
            batch.Draw(metal_sprite, pos_2, null, passive_color, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
            batch.DrawString(_font, metal_spears_text, new Vector2(pos_2.X, pos_2.Y), passive_font_color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.985f);
        }

        if (_controller.Selected == 2) // Wooden Spears at position 1 (left)
        {
            batch.Draw(_spears_background, pos_1, null, active_color, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
            batch.Draw(wooden_sprite, pos_1, null, active_color, 0f, Vector2.Zero, SpriteEffects.None, 0.995f);
            batch.DrawString(_font, wooden_spears_text, new Vector2(pos_1.X, pos_1.Y), active_font_color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
        else
        {
            batch.Draw(_spears_background, pos_1, null, passive_color, 0f, Vector2.Zero, SpriteEffects.None, 0.97f);
            batch.Draw(wooden_sprite, pos_1, null, passive_color, 0f, Vector2.Zero, SpriteEffects.None, 0.98f);
            batch.DrawString(_font, wooden_spears_text, new Vector2(pos_1.X, pos_1.Y), passive_font_color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.95f);
        }

        // Draw controls
        var controlButton = _controls_x;
        if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed) {
            controlButton = _controls_x_pressed;
        }

        var controlsSize = 48;
        var controlYMargin = 45;
        var controlRect = new Rectangle(pos_2.X + (int)((pos_2.Width - controlsSize) / 2f), pos_2.Y - controlYMargin, controlsSize, controlsSize);
        batch.Draw(controlButton, controlRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        
        var lbRect = new Rectangle(pos_1.X + (int)((pos_1.Width - controlsSize) / 2f), pos_1.Y - controlYMargin, controlsSize, controlsSize);
        batch.Draw(_controls_lb, lbRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        
        var rbRect = new Rectangle(pos_3.X + (int)((pos_3.Width - controlsSize) / 2f), pos_3.Y - controlYMargin, controlsSize, controlsSize);
        batch.Draw(_controls_rb, rbRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
    }
}