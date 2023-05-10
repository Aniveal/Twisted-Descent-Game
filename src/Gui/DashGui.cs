using System;
using System.Diagnostics;
using Meridian2.GameElements;
using Meridian2.Theseus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Meridian2.Gui;

public class DashGui : DrawableGameElement
{
    private readonly GameData _data;
    private readonly RopeGame _game;

    private Texture2D _running_icon;
    private Texture2D _active_running_icon;
    private Texture2D _dash_bar_background;
    private Texture2D _dash_bar;

    public DashGui(RopeGame game, GameData data)
    {
        _game = game;
        _data = data;
    }

    public void LoadContent()
    {
        _running_icon = _game.Content.Load<Texture2D>("Sprites/UI/running_icon");
        _active_running_icon = _game.Content.Load<Texture2D>("Sprites/UI/activate_running_icon");
        _dash_bar_background = _game.Content.Load<Texture2D>("Sprites/UI/dash_bar_background");
        _dash_bar = _game.Content.Load<Texture2D>("Sprites/UI/dash_bar");
    }


    public override void Draw(GameTime gameTime, SpriteBatch batch, Camera camera)
    {
        var viewportHeight = _game.GraphicsDevice.Viewport.Height;
        var margin = 10;                                                // number of pixels, distance to the bottom / the left of the screen
        var reload_duration = 5000;

        var dashbar_width = 200;
        var dashbar_heigth = (int)(2 * dashbar_width / 5);              // width to height fraction is 2 : 5

        var position = new Rectangle(
            margin,                                                     // draw dashbar [margin] many pixels away from the left side of the screen
            viewportHeight - margin - dashbar_heigth,                   // draw it [margin] many pixels away from the bottom of the screen
            dashbar_width,                                              // width set above
            dashbar_heigth                             
            );

        var dash_timer = _game._gameScreen.TheseusManager.Player.DashTimer;
        Debug.WriteLine(dash_timer);

        var dashbar_position = new Rectangle(
            margin + dashbar_width / 5,                                     // dashbar (moving part) starts at a 1/5th offset of the full dashbar visuals
            viewportHeight - margin - dashbar_heigth,                       // same vertical position as the other sprites
            (int)(4 * (dashbar_width * dash_timer) / (5 * reload_duration)),// compute length 
            dashbar_heigth                                                  // same height
            );


        batch.Draw(_dash_bar_background, position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);

        if (dash_timer >= reload_duration) // draw the running icon blue if a dash is available
        {
            batch.Draw(_active_running_icon, position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        } else
        {
            batch.Draw(_running_icon, position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
        
        batch.Draw(_dash_bar, dashbar_position, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.9f);
    }

}


// ignore first 2 / 10 of dash bar

