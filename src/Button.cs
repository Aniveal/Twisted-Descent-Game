using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Meridian2; 

public class Button : Component {
    #region Fields

    private MouseState _currentMouse;

    private readonly SpriteFont _font;

    private bool _isHovering;

    private MouseState _previousMouse;

    private readonly Texture2D _texture;

    #endregion

    #region Properties

    public event EventHandler Click;

    public bool Clicked { get; }

    public Color PenColour { get; set; }

    public Vector2 Position { get; set; }

    public Rectangle Rectangle => new((int)Position.X, (int)Position.Y, 480, 90);

    public string Text { get; set; }
    
    public bool Disabled { get; set; }

    #endregion

    #region Methods

    public Button(Texture2D texture, SpriteFont font) {
        _texture = texture;

        _font = font;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
        var colour = Color.Black; // Color.White;
        var font_colour = new Color(154, 134, 129);

        if (_isHovering)
            colour = Color.White; // Color.Gray;


        if (Disabled)
        {
            colour = new Color(Color.Gray, 0.2f);
            font_colour = new Color(110, 99, 97);
        }

        if (_isHovering)
        {
            // Drawing the Selection Highlight (red background behind text)
            // Selection Highlight is drawn 1/6 longer than text on each side (on the x-axis, y-axis stays the same)
            var x = (int)(Rectangle.X + Rectangle.Width - 8 * _font.MeasureString(Text).X / 6);
            int sprite_length = (int)(8 * _font.MeasureString(Text).X / 6);
            spriteBatch.Draw(_texture, new Rectangle(x, Rectangle.Y, sprite_length, 90), colour);
        }


        if (!string.IsNullOrEmpty(Text))
        {
            // drawing the text, shifting it 1/6 to the left s.t. it is centered in the selection highlight
            var x = Rectangle.X + Rectangle.Width - 7 * _font.MeasureString(Text).X / 6;
            var y = Rectangle.Y + Rectangle.Height / 2 - _font.MeasureString(Text).Y / 2;

            spriteBatch.DrawString(_font, Text, new Vector2(x, y), font_colour);
        }
    }

    public override void Update(GameTime gameTime) {
        _previousMouse = _currentMouse;
        _currentMouse = Mouse.GetState();

        var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

        _isHovering = false;

        if (mouseRectangle.Intersects(Rectangle)) {
            _isHovering = true;

            if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed && !Disabled)
                Click?.Invoke(this, new EventArgs());
        }
    }

    #endregion
}