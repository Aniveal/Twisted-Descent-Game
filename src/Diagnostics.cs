﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2 {
    public sealed class Diagnostics {
        private static readonly Diagnostics instance = new Diagnostics();

        static Diagnostics() {
        }

        private Diagnostics() {
        }

        public static Diagnostics Instance {
            get {
                return instance;
            }
        }

        private double _frames;
        private double _elapsed;
        private double _last;
        private double _now;
        private const double MsgFrequency = 1.0f;
        private string _fpsMessage = "";
        private double _force;
        private string _forceMessage = "";
        
        public void SetForce(double force) {
            _force = force;
        }

        public void Update(GameTime gameTime) {
            _now = gameTime.TotalGameTime.TotalSeconds;
            _elapsed = (_now - _last);
            if (_elapsed > MsgFrequency) {
                _fpsMessage = "FPS: " + (int)(_frames / _elapsed);
                _elapsed = 0;
                _frames = 0;
                _last = _now;
            }

            _forceMessage = "Force: " + _force;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor) {
            spriteBatch.DrawString(font, _fpsMessage, fpsDisplayPosition, fpsTextColor, 0f, Vector2.Zero, 0.7f,
                SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, _forceMessage, fpsDisplayPosition + new Vector2(0, 15), fpsTextColor, 0f,
                Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
            _frames++;
        }
    }
}