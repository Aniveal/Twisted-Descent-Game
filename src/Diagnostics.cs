using System;
using System.Drawing;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwistedDescent.Theseus;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TwistedDescent; 

public sealed class Diagnostics {
    private const double MsgFrequency = 1.0f;
    private string _dashMessage = "";
    private double _elapsed;
    private double _force;
    private string _forceMessage = "";
    public int Fps = 0;
    private string _fpsMessage = "";

    private double _frames;
    private double _last;
    private double _now;

    private int _widthOfRect = 0;

    static Diagnostics() { }

    private Diagnostics() { }

    public static Diagnostics Instance { get; } = new();


    public void SetForce(double force) {
        _force = force;
    }

    public void Update(GameTime gameTime, Player player) {
        _now = gameTime.TotalGameTime.TotalSeconds;
        _elapsed = _now - _last;
        if (_elapsed > MsgFrequency) {
            Fps = (int)(_frames / _elapsed);
            _fpsMessage = "FPS: " + Fps;
            _elapsed = 0;
            _frames = 0;
            _last = _now;
        }

        _forceMessage = "Joint Force: " + $"{_force,6:##0.00}";
        _dashMessage = "Dash Cooldown ";
        _widthOfRect = ((int)(Math.Round(player.DashTimer, 0))) * 100 / 5000 + 1;
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor) {
        spriteBatch.DrawString(font, _fpsMessage, fpsDisplayPosition, fpsTextColor, 0f, Vector2.Zero, 0.7f,
            SpriteEffects.None, 1f);
        spriteBatch.DrawString(font, _forceMessage, fpsDisplayPosition + new Vector2(0, 15), fpsTextColor, 0f,
            Vector2.Zero, 0.7f, SpriteEffects.None, 1f);

        if (_widthOfRect > 0) {
            Color[] data = new Color[14 * _widthOfRect];
            Texture2D rectTexture = new Texture2D(graphicsDevice, _widthOfRect, 14);

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.Red;

            rectTexture.SetData(data);
            var position = fpsDisplayPosition + new Vector2(100, 35);
            spriteBatch.Draw(rectTexture, position, null, Color.Red, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
        }

        DrawEmptyRectangle(spriteBatch, new Rectangle((int)fpsDisplayPosition.X + 100, (int)(fpsDisplayPosition.Y + 35), 70, 10), Color.Red, 5);

        spriteBatch.DrawString(font, _dashMessage, fpsDisplayPosition + new Vector2(0, 35), fpsTextColor, 0f,
            Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
        _frames++;
    }

    public static void DrawEmptyRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
    {
        Texture2D _pointTexture;   
        _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        _pointTexture.SetData<Color>(new Color[] { Color.White });

        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), null, color, 0f, Vector2.Zero
            , SpriteEffects.None, 1f);
        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), null, color, 0f, Vector2.Zero
            , SpriteEffects.None, 1f);
        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), null, color, 0f, Vector2.Zero
            , SpriteEffects.None, 1f);
        spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), null, color, 0f, Vector2.Zero
            , SpriteEffects.None, 1f);
    }
}