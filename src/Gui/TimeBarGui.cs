using System.Diagnostics;
using Meridian2.GameElements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui;

public class TimeBarGui : DrawableGameElement
{
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _sandglas_background;     // background, sandglas without sand in it
    private Texture2D _sandglas_upper_sand;      // sand in the upper half of the sandglas, decreases with time
    private Texture2D _sandglas_lower_sand;      // sand in the upper half of the sandglas, increases with time
    private Texture2D _sandglas_light_effect;   // should be drawn in front of sandglas, "reflections" to make it look like glas

    private SpriteFont _font;

    // old
    // The bar is 80% of screen height
    private const float BarHeightRatio = 0.8f;
    private const float FillBarWidthRatio = 0.7f;
    private const int Margin = 10;

    private Texture2D _frontTexture;
    private Texture2D _backTexture;
    private Texture2D _fillTexture;

    private readonly Color _fullColor = new(68, 156, 212);
    private readonly Color _emptyColor = new(229, 73, 73);
    //


    public TimeBarGui(RopeGame game, GameData data)
    {
        _game = game;
        _data = data;
    }

    public void LoadContent()
    {
        _sandglas_background = _game.Content.Load<Texture2D>("Sprites/UI/sandglas_background");
        _sandglas_upper_sand = _game.Content.Load<Texture2D>("Sprites/UI/sandglas_upper_sand");
        _sandglas_lower_sand = _game.Content.Load<Texture2D>("Sprites/UI/sandglas_lower_sand");
        _sandglas_light_effect = _game.Content.Load<Texture2D>("Sprites/UI/sandglas_light_effect");

        _font = _game.Content.Load<SpriteFont>("damn_ui");
    }

    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        var viewportWidth = _game.GraphicsDevice.Viewport.Width;
        var viewportHeight = _game.GraphicsDevice.Viewport.Height;

        var margin = 10;
        var hourglas_width = 40;
        var hourglas_height = 2 * hourglas_width;
        var hourglas_maxtime = _data.MaxTimeLeft;

        // Drawing the remaining time in seconds as text:

        var text = ((int)_data.TimeLeft).ToString();
        var textSize = _font.MeasureString(text);
        var text_position = new Vector2(viewportWidth - margin - textSize.X, margin);

        var normal_color = new Color(154, 139, 141);
        var red_color = new Color(170, 54, 54);

        var stress_factor = (float)(_data.TimeLeft / 30);            // reaches full red value at 10 seconds, transitions for 30 seconds
        var text_color = Color.Lerp(red_color, normal_color, stress_factor);

        batch.DrawString(_font, text, text_position, text_color, 0, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        // Drawing the background hourglas:
        var sprite_color = Color.Lerp(red_color, Color.White, stress_factor);

        var hourglas_position = new Rectangle(
            viewportWidth - margin - hourglas_width,
            margin + (int)textSize.Y,
            hourglas_width,
            hourglas_height
            );

        batch.Draw(_sandglas_background, hourglas_position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.99f);


        // Drawing the upper sand:
        var upper_sand_height = (int)((7 * hourglas_height * _data.TimeLeft) / (16 * hourglas_maxtime));

        var upper_sand_position = new Rectangle(
            hourglas_position.X,
            hourglas_position.Y + hourglas_position.Height / 2 - upper_sand_height,
            hourglas_width,
            upper_sand_height
            );
        batch.Draw(_sandglas_upper_sand, upper_sand_position, null, sprite_color, 0f, Vector2.Zero, SpriteEffects.None, 0.995f);

        // Drawing the lower sand:
        var lower_sand_height = (int)((7 * hourglas_height * (hourglas_maxtime - _data.TimeLeft)) / (16 * hourglas_maxtime));
        var lower_sand_position = new Rectangle(
            hourglas_position.X,
            hourglas_position.Y + 15 * hourglas_position.Height / 16 - lower_sand_height,
            hourglas_width,
            lower_sand_height
            );
        batch.Draw(_sandglas_lower_sand, lower_sand_position, null, sprite_color, 0f, Vector2.Zero, SpriteEffects.None, 0.995f);

        batch.Draw(_sandglas_light_effect, hourglas_position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

    }
}