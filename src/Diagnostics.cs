using System;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2; 

public sealed class Diagnostics {
    private const double MsgFrequency = 1.0f;
    private string _dashMessage = "";
    private double _elapsed;
    private double _force;
    private string _forceMessage = "";
    private string _fpsMessage = "";

    private double _frames;
    private double _last;
    private double _now;

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
            _fpsMessage = "FPS: " + (int)(_frames / _elapsed);
            _elapsed = 0;
            _frames = 0;
            _last = _now;
        }

        _forceMessage = "Joint Force: " + $"{_force,6:##0.00}";
        _dashMessage = "Dash Cooldown: " + (5000 - Math.Round(player.DashTimer, 0));
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor) {
        spriteBatch.DrawString(font, _fpsMessage, fpsDisplayPosition, fpsTextColor, 0f, Vector2.Zero, 0.7f,
            SpriteEffects.None, 1f);
        spriteBatch.DrawString(font, _forceMessage, fpsDisplayPosition + new Vector2(0, 15), fpsTextColor, 0f,
            Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
        spriteBatch.DrawString(font, _dashMessage, fpsDisplayPosition + new Vector2(0, 35), fpsTextColor, 0f,
            Vector2.Zero, 0.7f, SpriteEffects.None, 1f);
        _frames++;
    }
}