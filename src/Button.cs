using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwistedDescent; 

public class Button : Component {
    #region Fields

    private MouseState _currentMouse;

    private readonly SpriteFont _font;

    private bool _isHovering;

    private bool _isMouseControlled;

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
            spriteBatch.Draw(_texture, new Rectangle(Rectangle.X, Rectangle.Y, (int)_font.MeasureString(Text).X + 80, Rectangle.Height), colour);
        }


        if (!string.IsNullOrEmpty(Text))
        {
            // drawing the text, shifting it by 40 pixels to the left s.t. it is centered in the selection highlight
            var x = Rectangle.X + 40;
            var y = Rectangle.Y;

            spriteBatch.DrawString(_font, Text, new Vector2(x, y), font_colour);
        }
    }

    public void Trigger() {
        SoundEngine.Instance.ButtonClick();
        Click?.Invoke(this, new EventArgs());
    }

    public void SetHover(bool hover) {
        _isHovering = hover;
        _isMouseControlled = false;
    }

    public override void Update(GameTime gameTime) {
        _previousMouse = _currentMouse;
        _currentMouse = Mouse.GetState();

        // Enable mouse controls if mouse moved
        if (_currentMouse.Position != _previousMouse.Position) {
            _isMouseControlled = true;
        }
        
        if (_isMouseControlled) {
            var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle)) {
                _isHovering = true;

                if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed && !Disabled)
                    Trigger();
            }
        }
    }

    #endregion
}