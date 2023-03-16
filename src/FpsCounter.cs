using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2
{
    public class FpsCounter
    {
        private double _frames = 0;
        private double _updates = 0;
        private double _elapsed = 0;
        private double _last = 0;
        private double _now = 0;
        public const double MsgFrequency = 1.0f;
        public string Msg = "";

        public void Update(GameTime gameTime)
        {
            _now = gameTime.TotalGameTime.TotalSeconds;
            _elapsed = (double)(_now - _last);
            if (_elapsed > MsgFrequency) {
                Msg = " Fps: " + (int)(_frames / _elapsed); // + "\n Elapsed time: " + _elapsed +  "\n Updates: " + _updates + "\n Frames: " + _frames;
                _elapsed = 0;
                _frames = 0;
                _updates = 0;
                _last = _now;
            }
            _updates++;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor)
        {
            spriteBatch.DrawString(font, Msg, fpsDisplayPosition, fpsTextColor, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
            _frames++;
        }
    }
}